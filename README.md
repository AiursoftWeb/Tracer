# Tracer

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/aiursoftweb/tracer/blob/master/LICENSE)
[![Pipeline stat](https://gitlab.aiursoft.com/aiursoft/tracer/badges/master/pipeline.svg)](https://gitlab.aiursoft.com/aiursoft/tracer/-/pipelines)
[![Test Coverage](https://gitlab.aiursoft.com/aiursoft/tracer/badges/master/coverage.svg)](https://gitlab.aiursoft.com/aiursoft/tracer/-/pipelines)
[![S360 compliance](https://s360.aiursoft.com/badge.svg?project=aiursoft%2Ftracer)](https://s360.aiursoft.com/SupplyChain?tab=projects&project=aiursoft%2Ftracer&scope=baseline&ecosystem=all)
[![Man hours](https://manhours.aiursoft.com/r/github.com/aiursoftweb/tracer.svg)](https://manhours.aiursoft.com/r/github.com/aiursoftweb/tracer.html)
[![Website](https://img.shields.io/website?url=https%3A%2F%2Ftracer.aiursoft.com%2F)](https://tracer.aiursoft.com)
[![Docker](https://img.shields.io/docker/pulls/aiursoft/tracer.svg)](https://hub.docker.com/r/aiursoft/tracer)

Tracer is a simple network speed test app. Deploy this on your server. Open your tracer on your browser. And you can test the speed between you and your server.

![overview](./screenshot.png)

## Try

Try a running tracer [here](https://tracer.aiursoft.com).

## Install with Docker

Run the installer with the desired port (omit it to choose an available port):

```bash
curl -fsSL https://raw.githubusercontent.com/aiursoftweb/tracer/master/install.sh | sudo bash -s -- 8080
```

Installation starts after three seconds; press Ctrl+C to cancel. Then open `http://localhost:8080`.

## Run manually

Requirements about how to run

1. Install [.NET 10 SDK](http://dot.net/) and [Node.js](https://nodejs.org/).
2. Execute `npm install` at `wwwroot` folder to install the dependencies.
3. Execute `dotnet run` to run the app.
4. Use your browser to view [http://localhost:5000](http://localhost:5000).

## Run in Microsoft Visual Studio

1. Open the `.sln` file in the project path.
2. Press `F5` to run the app.

## Run in Docker

First, install Docker [here](https://docs.docker.com/get-docker/).

Then run the following commands in a Linux shell:

```bash
image=aiursoft/tracer
appName=tracer
docker pull $image
docker run -d --name $appName --restart unless-stopped -p 5000:5000 -v /var/www/$appName:/data $image
```

That will start a web server at `http://localhost:5000` and you can test the app.

The docker image has the following context:

| Properties  | Value                           |
|-------------|---------------------------------|
| Image       | aiursoft/tracer                 |
| Ports       | 5000                            |
| Binary path | /app                            |
| Data path   | /data                           |
| Config path | /data/appsettings.json          |

## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you with push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your workflow cruft out of sight.

We're also interested in your feedback on the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.
