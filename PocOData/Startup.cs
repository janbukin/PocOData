﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PocOData.Startup))]

namespace PocOData
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
