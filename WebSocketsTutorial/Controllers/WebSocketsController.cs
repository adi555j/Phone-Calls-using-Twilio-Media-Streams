using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace WebSocketsAudioStreaming.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketsController : ControllerBase
    {
        private readonly ILogger<WebSocketsController> _logger;

        public WebSocketsController(ILogger<WebSocketsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Websocket requests should be routed here for streameing
        /// </summary>
        /// <returns></returns>
        ///

        [HttpGet("/ws")]
        public async Task Get()
        {
          if (HttpContext.WebSockets.IsWebSocketRequest)
          {
              using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
              _logger.Log(LogLevel.Information, "WebSocket connection established");
              await Echo(webSocket);
          }
          else
          {
              HttpContext.Response.StatusCode = 400;
          }
        }

        /// <summary>
        /// This method just returns the websocket URL for audio stream (/ws)
        /// </summary>
        /// <returns></returns>
        ///


        [HttpPost("/stream")]
        public async Task<ContentResult> Stream()
        {
            
            string res = "<Response> <Start><Stream url=\"" + "wss://"+ "01153716374a.ngrok.io/ws/" + "\" /></Start><Say>I will stream the next 60 seconds of audio through your websocket</Say><Pause length="+ "\"30\"" + "/></Response> ";

            return Content(res, "text/xml", Encoding.UTF8);

        }


        private async Task Echo(WebSocket webSocket)
        {
            
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            

            _logger.Log(LogLevel.Information, "Message received from Client");

            while (!result.CloseStatus.HasValue)
            {
                var serverMsg = Encoding.UTF8.GetBytes($"Server: Hello. You said: {Encoding.UTF8.GetString(buffer)}");
                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message sent to Client");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message received from Client");
                
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _logger.Log(LogLevel.Information, "WebSocket connection closed");
        }
    }
}
