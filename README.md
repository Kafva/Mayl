# Mayler

## [server-side](https://developers.google.com/gmail/api/auth/web-server)
We will need server side authorization with a refresh token to avoid being logged-in to google in our browser session, we will however still haft to login once to authorize the app.

Set the app as a "Web server" and download `credentials.json` from the credentials tab.

Setup backend that fetches emails and displays them in a webview
Use REST endpoint to delete mail
