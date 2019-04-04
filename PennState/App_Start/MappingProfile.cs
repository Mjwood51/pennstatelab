using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PennState.Models;

namespace PennState.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<User, Tbl_Users>();
            Mapper.CreateMap<Tbl_Users, User>();

            Mapper.CreateMap<CatagoryLocation, Tbl_CatagoryLocations>();
            Mapper.CreateMap<Tbl_CatagoryLocations, CatagoryLocation>();

            Mapper.CreateMap<CatagoryOwner, Tbl_CatagoryOwners>();
            Mapper.CreateMap<Tbl_CatagoryOwners, CatagoryOwner>();

            Mapper.CreateMap<CatagoryType, Tbl_CatagoryTypes>();
            Mapper.CreateMap<Tbl_CatagoryTypes, CatagoryType>();

            Mapper.CreateMap<CatagoryVendor, Tbl_CatagoryVendors>();
            Mapper.CreateMap<Tbl_CatagoryVendors, CatagoryVendor>();

            Mapper.CreateMap<Files, Tbl_File>();
            Mapper.CreateMap<Tbl_File, Files>();

            Mapper.CreateMap<Item, Tbl_Items>();
            Mapper.CreateMap<Tbl_Items, Item>();

            Mapper.CreateMap<Location, Tbl_Locations>();
            Mapper.CreateMap<Tbl_Locations, Location>();

            Mapper.CreateMap<SubLocation, Tbl_SubLocations>();
            Mapper.CreateMap<Tbl_SubLocations, SubLocation>();
           
            Mapper.CreateMap<Photos, Tbl_Photo>();
            Mapper.CreateMap<Tbl_Photo, Photos>();

            Mapper.CreateMap<Role, Tbl_Roles>();
            Mapper.CreateMap<Tbl_Roles, Role>();

            Mapper.CreateMap<User, Tbl_Users>();
            Mapper.CreateMap<Tbl_Users, User>();
        }
    }
}