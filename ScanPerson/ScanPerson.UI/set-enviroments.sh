# build.sh
   #!/bin/bash
   echo "Current directory: $(pwd)"
   echo "Files in directory: $(ls -la src/enviroments)"
   sed -i "s|scanperson.auth.api|$SCANPERSON_AUTH_API_DOMEN|g" src/enviroments/enviroments.ts
   sed -i "s|scanperson.webapi|$SCANPERSON_WEBAPI_DOMEN|g" src/enviroments/enviroments.ts
   sed -i "s|aspnetcore.enviroment|$ASPNETCORE_ENVIRONMENT|g" src/enviroments/enviroments.ts
