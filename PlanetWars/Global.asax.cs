using AutoMapper;
using PlanetWars.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PlanetWars
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Configure AutoMapper
            Mapper.CreateMap<Server.Planet, Shared.Planet>().IgnoreAllNonExisting();
            Mapper.CreateMap<Server.Fleet, Shared.Fleet>().ForMember(
                dest => dest.DestinationPlanetId,
                opt => opt.MapFrom(src => src.Destination.Id)).ForMember(
                dest => dest.SourcePlanetId,
                opt => opt.MapFrom(src => src.Source.Id)).IgnoreAllNonExisting();


            Mapper.AssertConfigurationIsValid();
        }
    }
}
