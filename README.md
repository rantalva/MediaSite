# MediaSite
this is a fullstack application media site built for publishing articles.

Goal is to create a search engine optimized article/news site with role based permissions.

## Stack
#### Database
For database, Postgresql 18 is used
#### Backend
.Net ASP NET Core web API
- postgresql entitycore library
- Identity Core to handle users and user roles
- Logging needs to be added
#### Frontend
Frontend will be build with Astro due content mostly being static.
- Frontend will in future have a rich texteditor for creating posts, this will be done with Lexical
#### Hosting
Most likely a VPS will be used for hosting, but due to price increase this is still open. We could use Cloud provider like AWS/Azure, but their costs also high and maybe overkill

Future stack:
- VPS (Hetzner?)
- Github actions
- Docker