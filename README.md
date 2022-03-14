We plan to use C# rather than node as described in https://www.twilio.com/blog/live-transcribing-phone-calls-using-twilio-media-streams-and-google-speech-text



* Line 55 - WebSocketController.cs
Replace wss://"+ "01153716374a.ngrok.io/ws/ with your own WSS url.

* Update your Twilio Phone Voice URL
twilio phone-numbers:update "+1XXXXXXX" --voice-url  https://ngrok_url/stream

* Run ngrok like this if you are testing locally - I am assuming you are using port 5001
ngrok http https://localhost:5001 -host-header="localhost:5001"






