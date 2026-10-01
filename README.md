# MediaSite
this is a fullstack application media site built for publishing articles.

Goal is to create a search engine optimized article/news site with role based permissions.

For secrets use Infisical in future

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
- https://webcoreui.dev/blocks/introduction
#### Hosting
Most likely a VPS will be used for hosting, but due to price increase this is still open. We could use Cloud provider like AWS/Azure, but their costs also high and maybe overkill

Future stack:
- VPS (Hetzner?) mostlikely will be upcloud due to it having local vps.
- Github actions
- Docker


## Infisical Setup

Infisical provides the application's secrets without storing them in the repository.

### Setup

1. **Go to Infisical**
   - Open the MediaSite project.
   - Create a **Machine Identity** with read access to the required environment.

2. **Create the credentials**
   - Copy the Machine Identity's:
     - `Client ID`
     - `Client Secret`

3. **Add them to the machine's environment**

Windows:

    ```powershell
    [Environment]::SetEnvironmentVariable("INFISICAL_CLIENT_ID", "YOUR_CLIENT_ID", "User")
    [Environment]::SetEnvironmentVariable("INFISICAL_CLIENT_SECRET", "YOUR_CLIENT_SECRET", "User")

Linux:

    ```Bash   
    export INFISICAL_CLIENT_ID="YOUR_CLIENT_ID"
    export INFISICAL_CLIENT_SECRET="YOUR_CLIENT_SECRET"
