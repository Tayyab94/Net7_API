using AutoMapper;
using LearnAPI_Net7.Models;
using LearnAPI_Net7.Models.ViewModels;

namespace LearnAPI_Net7.Helpers
{
    public class AutoMapperHandler :Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<Customer, CustomerVM>().ForMember(item => item.Info, opt=>opt.MapFrom(s=>
            $"{s.Id}-{s.Name}-{s.Email}"));
        }
    }
}
