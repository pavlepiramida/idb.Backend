# Backend for idb

Home of backend implementation for [idb](https://github.com/vladgr/idb).  
Implemented as ASP.Net Web API, using MongoDB as database engine.  
Aims to make the backend easily deployable for personal/teamwork usage.

# Motivation

After searching for FOSS note taking app that fits my use case,  
I'd given up on them and started working on my own,better, with blackjack and bugs.  
That fever dream lasted for couple days until I realized what I need is too complicated to glue in few days,  
so naturally I gave up (also I'm pretty lazy).

Sometime ago,gods of Github smiled upon me and I came across [idb](https://github.com/vladgr/idb)  
which seems like something that fits well in my use case,  
so I decided to create a backend and make it Heroku friendly (keep it cheap as duck boi).

#### Here be dragons

-   Hastily written backend implementation (might go kaboom)
-   Functional parity at 80%
-   Will try to follow development of [idb](https://github.com/vladgr/idb) closely

# Getting started

### Requirements

-   .net 5
-   Visual Studio Code
    -   C# extensions
-   Visual Studio
-   Docker
    -   heroku-cli (optional, for hosting on Heroku)
-   MongoDB Atlas account (optional, MongoDB in the sky)

### TODO

-   Image storage
-   Unit tests (lel)
-   docker-compose for local usage
-   Documenting process of hosting and deployment to Heroku
    -   MongoDB Atlas creation and setup
-   120% functional parity
