const fs = require('fs');

   const proxyConfig = {
       "/webApi/*": {
           "target": "https://" + process.env.SCANPERSON_WEBAPI_DOMEN + ':8081',
           "secure": false,
           "changeOrigin": true,
           "logLevel": "debug"
       },
       "/authApi/*": {
           "target": "https://" + process.env.SCANPERSON_AUTH_API_DOMEN + ':8091',
           "secure": false,
           "changeOrigin": true,
           "logLevel": "debug"
       }
   };

   fs.writeFileSync('proxy.conf.json', JSON.stringify(proxyConfig, null, 2));
