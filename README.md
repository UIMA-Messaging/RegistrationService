# Registration Service

The Registration Service is responsible for handling user registations. This service is part of the greater UIMA project. It communicates between an API gateway by desktop clients and has a direct connection to an event bus. This service has its own database for storing user data:

![Individual  UIMA C4 Model](https://github.com/UIMA-Messaging/registration-service/assets/56337726/86ccd2b6-6d56-4a82-9193-845e11da7a87)

## Configuration
You can configure environment variable in appsettings.json file in the project. This file should look like:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Users": "POSTGRES_CONNECTION_STRING"
  },
  "Bugsnag": {
    "ApiKey": "BUGSNAG_API_KEY"
  },
  "RabbitMQ": {
    "HOST": "RABBITMQ_HOST",
    "Username": "RABBITMQ_USERNAME",
    "Password": "RABBITMQ_PASSWORD",
    "Exchange": "THIS_EXCHANGE"
  },
  "Jabber": {
    "Host": "JABBER_HOST"
  }
}
```

## Endpoints

The Registration Service has one controller for handling all user registrations.

> ⚠ All requests must be authenticated to reach the following endpoints

`POST users/register`

This endpoint is responsible for registering basic users. Should validation of data fail or a conflicting display name occur, a 400 reponse is returned along with a message. When a user is registered, hes registration is published to an event bus with routing key `users.new` and his exchange keys, with routing key `users.new.keys`.  

Request body:
```json
{
  "displayName": "QAQAQ",
  "image": "string",
  "exchangeKeys": {
    "userId": "string",
    "identityKey": "F91b9efd6DA9E5d0E6a5FaB5efa0f213D5DCF61aE89Ac1Ba6EFCBaB696A3D36",
    "signedPreKey": "7fb5fc05De3aE61EBeAb4d67E2a22cFD8ecaa4Cfaf00Fb89339F8eDCe9fF4A54DaaFc8aFfA91BA3fAe",
    "oneTimePreKeys": [
      "string",
      ...
      "string"
    ],
    "signature": "string"
  }
}
```

Response body:
```json
{
  "id": "string",
  "jid": "string",
  "displayName": "string",
  "username": "string",
  "image": "string",
  "ephemeralPassword": "string",
  "joinedAt": "2023-06-17T12:40:45.223Z",
  "editedAt": "2023-06-17T12:40:45.223Z"
}
```

`DELETE users/unregister/{user}`



