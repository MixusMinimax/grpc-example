﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using UserService;

CreateHostBuilder(args).Build().Run();

IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });