# Tracer

[![Build status](https://dev.azure.com/aiursoft/Star/_apis/build/status/Tracer%20Build)](https://dev.azure.com/aiursoft/Star/_build/latest?definitionId=1)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/aiursoft/Star/1)
![Website](https://img.shields.io/website?url=https%3A%2F%2Ftracer.aiursoft.com)

Tracer is a simple network speed test app. 

Tracer is a simple network speed test app. Deploy this on your own server. Open your tracer on your browser. And you can test the speed between you and your server.

## Try

Try a running tracer [here](https://tracer.aiursoft.com).

## Run in Ubuntu

First, specify a domain name for your Ubuntu 18.04+, brand new server.

And execute the following command in the server:

```bash
$ curl -sL https://github.com/AiursoftWeb/Tracer/raw/master/install.sh | sudo bash -s www.example.com
```

## Run locally

Requirements about how to run

* [.NET Core runtime 5.0 or later](https://github.com/dotnet/core/tree/master/release-notes)

Requirements about how to develope

* [.NET Core SDK 5.0 or later](https://github.com/dotnet/core/tree/master/release-notes)
* [VS Code](https://code.visualstudio.com) (Strongly suggest)

1. Excute `dotnet restore` to restore all dotnet requirements
2. Excute `dotnet run` to run the app
3. Use your browser to view [http://localhost:5000](http://localhost:5000)

## Run in Microsoft Visual Studio

1. Open the `.sln` file in the project path. 
2. Press `F5`.

## Run in docker

Pull the container using command:

```bash
$ docker pull anduin2019/tracer:1.0.0
$ docker run -d -p 8080:80 anduin2019/tracer:1.0.0
```

That will start a web server at `http://localhost:8080` and you can test the app.

## How to build locally in docker

Just install docker and docker-compose. Execute the following command.

```bash
$ docker build -t=tracer .
$ docker run -d -p 8080:80 tracer
```

That will start a web server at `http://localhost:8080` and you can test the app.

## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you have push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your personal workflow cruft out of sight.

We're also interested in your feedback for the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.
