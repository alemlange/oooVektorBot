using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using ManagerDesk.ViewModels;
using DataModels;

namespace ManagerDesk
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Mapper.Initialize(cfg => 
                {
                    cfg.CreateMap<Dish, DishViewModel>();
                    cfg.CreateMap<Table, TableCardViewModel>();
                    cfg.CreateMap<Menu, MenuViewModel>();
                    cfg.CreateMap<Restaurant, RestaurantViewModel>();
                    cfg.CreateMap<Restaurant, RestaurantDropDown>();
                    cfg.CreateMap<Dish, SelectedDishViewModel>();
                    cfg.CreateMap<Modificator, SelectedModViewModel>();
                    cfg.CreateMap<Modificator, ModificatorViewModel>();
                    cfg.CreateMap<Dispatch, DispatchViewModel>();
                    cfg.CreateMap<Feedback, FeedbackViewModel>();
                    cfg.CreateMap<Booking, BookingViewModel>();
                    cfg.CreateMap<Cheque, ChequeViewModel>();

                    cfg.CreateMap<Config, ConfigViewModel>();
                });
        }
    }
}
