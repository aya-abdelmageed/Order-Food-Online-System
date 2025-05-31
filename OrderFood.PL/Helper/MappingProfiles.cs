using AutoMapper;
using OrderFood.DAL.Entities.Models;
using OrderFood.DAL.Entities.User;
using OrderFood.PL.Areas.Delivery.ViewModel;

namespace OrderFood.PL.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Restaurant, RestaurantVM>();


            CreateMap<Order, DeliveryOrderDetailsVM>()
                .ForMember(des => des.AmountPercentageCoupon,
                            opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.AmountPercentage : 0))
                .ForMember(dovm => dovm.Customer,
                            opt => opt.MapFrom(src => src.Customer))
                .ForMember(dovm => dovm.AmountPercentageCoupon,
                            opt => opt.MapFrom(src => src.Coupon.AmountPercentage));




            CreateMap<OrderMeals, OrderMealsVM>()
                .ForMember(ovm=> ovm.Name, opt => opt.MapFrom(src => src.Meal.Name))
                .ForMember(ovm => ovm.Image, opt => opt.MapFrom(src => src.Meal.Image))
                .ForMember(ovm => ovm.Price, opt => opt.MapFrom(src => src.Meal.Price))
                .ForMember(ovm => ovm.Quantity, opt => opt.MapFrom(src => src.Quantity));


            CreateMap<ApplicationUser, UserVM>();
        }
    }
}
