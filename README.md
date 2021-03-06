# Mayl

## Google API Setup
1. Go to the [Google developer console](https://console.cloud.google.com/apis) and create a new project
2. Enable the Gmail API from the *Enable APIs and Services* button
3. Follow the steps given from the *OAuth consent screen* tab
	- Add all Gmail accounts that should be able to use the app as *Test users*
4. Next, create *OAuth client ID* credentials from the *Credentials* tab
	- *Type*: Web application
	- *Authorized redirect URIs*: `http://127.0.0.1/authorize/`
5. Download the credentials and place the file at `./secret/credentials.json`

## Add Gmail accounts
This section needs to be performed on a machine with a GUI to interact with browser prompts, if you want to deploy the application on a server copy the contents of the repository after this section and continue from there. Updating which accounts are available and renewing OAuth tokens requires this process to be redone. You should **NOT** deploy the app publicly since all authentication is performed during this step whereafter your account is available to the server. 

Deploying on a server also requires `options.Listen(IPAddress.Loopback, ...)` to be changed to `options.Listen(IPAddress.Any, ...)` in `Program.cs:36`.

Switch to `./RegisterAccount` and execute 
```
dotnet run <your-account@gmail.com> 
```
This will produce an authentication prompt in your browser which upon successful completion will produce a token at `./secret/<your-account>_token/`. To verify that the connection was successfully established run (from `./RegisterAccount`)
```
dotnet run -t <your-account@gmail.com> 
```
If the connection was successful add the account to `./secret/accounts.txt` (newline separated), the first entry will be used as the default and the default label can be set in `./Client/src/config.js`.

*Note* that to add several accounts it is necessary to sign out from Google after each successful authentication in the browser that receives the prompt.

## TLS setup
Create a `server.key` and `server.crt` with `openssl` signed by a trusted CA and generate a `.pfx` file using the command
```bash
openssl pkcs12 -export -out ./secret/server.pfx -inkey server.key -in server.crt
```
Next, create the file `./secret/certificate.json` with the following content
```json
{
  "certificateSettings": {
    "fileName": "./secret/server.pfx",
    "password": "<password for server.pfx>"
  }
}
```

## Run the app
```bash
# Install dependencies for the client
npm install

# Build and start the server
dotnet run
```

## Unimplemented features
* Sending emails
* Access to attachments
