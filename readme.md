# Tracer

[![Build Status](https://travis-ci.org/Anduin2017/Tracer.svg?branch=master)](https://travis-ci.org/Anduin2017/Tracer)

Tracer is a simple network speed test app

## Requirements

Requirements about how to run
* [Windows Server](http://www.microsoft.com/en-us/cloud-platform/windows-server) or [Ubuntu Server](https://www.ubuntu.com/server)
* [dot net Core 2.0.0 or later](https://github.com/dotnet/core/tree/master/release-notes)
* [git](https://git-scm.com)

**bower depends on nodejs, npm and git!**

Requirements about how to develope
* [Windows 10](http://www.microsoft.com/en-US/windows/) or [Ubuntu desktop](https://www.ubuntu.com/desktop)
* [dot net Core SDK 2.0.0 or later](https://github.com/dotnet/core/tree/master/release-notes)
* [git](https://git-scm.com)
* [VS Code](https://code.visualstudio.com) (Strongly suggest)

## How to run locally

1. Excute `bower install` to download all front-end packages
2. Excute `dotnet restore` to restore all dotnet requirements
3. Excute `dotnet run` to run the app
4. Use your browser to view [http://localhost:5000](http://localhost:5000)

## How to run in docker

Just install docker and docker-compose. Execute the following command.

```bash
$ docker-compose build
$ docker-compose up
```

That will start a web server at `http://localhost:8000` and you can test the app.

## How to publish to your server

1. Prepare a Linux or Windows Server
2. Run dotnet publish from your dev environment to package your app into a self-contained directory that can run on your server.
3. Copy your ASP.NET Core app to your server using whatever tool (SCP, FTP, etc) integrates into your workflow. Test your app, for example:
+ 
From the command line, run dotnet yourapp.dll
In a browser, navigate to http://<serveraddress>:<port> to verify you app works on Linux. 
+ 

> Note: You can use Yeoman to create a new ASP.NET Core application for a new project.

**When you have successfully published the app to your server, it is not running properly! You need the following setps.**
