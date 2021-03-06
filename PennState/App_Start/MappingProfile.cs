﻿using AutoMapper;
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
            Mapper.CreateMap<User, Tbl_Users>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.ActivationCode, property => property.MapFrom(i => i.ActivationCode))
                .ForMember(u => u.Email, property => property.MapFrom(i => i.Email))
                .ForMember(u => u.FirstName, property => property.MapFrom(i => i.FirstName))
                .ForMember(u => u.IsActive, property => property.MapFrom(i => i.IsActive))
                .ForMember(u => u.Tbl_Items, property => property.MapFrom(i => i.Items))
                .ForMember(u => u.LastName, property => property.MapFrom(i => i.LastName))
                .ForMember(u => u.PasswordHashed, property => property.MapFrom(i => i.PasswordHashed))
                .ForMember(u => u.RoleId, property => property.MapFrom(i => i.RoleId))
                .ForMember(u => u.Tbl_Roles, property => property.MapFrom(i => i.Roles))
                .ForMember(u => u.UserName, property => property.MapFrom(i => i.UserName));

            Mapper.CreateMap<Tbl_Users, User>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.ActivationCode, property => property.MapFrom(i => i.ActivationCode))
                .ForMember(u => u.Email, property => property.MapFrom(i => i.Email))
                .ForMember(u => u.FirstName, property => property.MapFrom(i => i.FirstName))
                .ForMember(u => u.IsActive, property => property.MapFrom(i => i.IsActive))
                .ForMember(u => u.Items, property => property.MapFrom(i => i.Tbl_Items))
                .ForMember(u => u.LastName, property => property.MapFrom(i => i.LastName))
                .ForMember(u => u.PasswordHashed, property => property.MapFrom(i => i.PasswordHashed))
                .ForMember(u => u.RoleId, property => property.MapFrom(i => i.RoleId))
                .ForMember(u => u.Roles, property => property.MapFrom(i => i.Tbl_Roles))
                .ForMember(u => u.UserName, property => property.MapFrom(i => i.UserName));

            Mapper.CreateMap<CatagoryLocation, Tbl_CatagoryLocations>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Tbl_CatagoryLocations1, property => property.MapFrom(i => i.Childs))
                .ForMember(u => u.HasChildren, property => property.MapFrom(i => i.HasChildren))
                .ForMember(u => u.LocationName, property => property.MapFrom(i => i.LocationName))
                .ForMember(u => u.Tbl_CatagoryLocations2, property => property.MapFrom(i => i.Parent))
                .ForMember(u => u.Pid, property => property.MapFrom(i => i.Pid));



            Mapper.CreateMap<Tbl_CatagoryLocations, CatagoryLocation>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Childs, property => property.MapFrom(i => i.Tbl_CatagoryLocations1))
                .ForMember(u => u.HasChildren, property => property.MapFrom(i => i.HasChildren))
                .ForMember(u => u.LocationName, property => property.MapFrom(i => i.LocationName))
                .ForMember(u => u.Parent, property => property.MapFrom(i => i.Tbl_CatagoryLocations2))
                .ForMember(u => u.Pid, property => property.MapFrom(i => i.Pid));

            Mapper.CreateMap<CatagoryOwner, Tbl_CatagoryOwners>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Tbl_CatagoryOwners1, property => property.MapFrom(i => i.Childs))
                .ForMember(u => u.HasChildren, property => property.MapFrom(i => i.HasChildren))
                .ForMember(u => u.OwnerName, property => property.MapFrom(i => i.OwnerName))
                .ForMember(u => u.Tbl_CatagoryOwners2, property => property.MapFrom(i => i.Parent))
                .ForMember(u => u.Pid, property => property.MapFrom(i => i.Pid));

            Mapper.CreateMap<Tbl_CatagoryOwners, CatagoryOwner>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Childs, property => property.MapFrom(i => i.Tbl_CatagoryOwners1))
                .ForMember(u => u.HasChildren, property => property.MapFrom(i => i.HasChildren))
                .ForMember(u => u.OwnerName, property => property.MapFrom(i => i.OwnerName))
                .ForMember(u => u.Parent, property => property.MapFrom(i => i.Tbl_CatagoryOwners2))
                .ForMember(u => u.Pid, property => property.MapFrom(i => i.Pid));

            Mapper.CreateMap<CatagoryType, Tbl_CatagoryTypes>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Tbl_CatagoryTypes1, property => property.MapFrom(i => i.Childs))
                .ForMember(u => u.HasChildren, property => property.MapFrom(i => i.HasChildren))
                .ForMember(u => u.TypeName, property => property.MapFrom(i => i.TypeName))
                .ForMember(u => u.Tbl_CatagoryTypes2, property => property.MapFrom(i => i.Parent))
                .ForMember(u => u.Pid, property => property.MapFrom(i => i.Pid));

            Mapper.CreateMap<Tbl_CatagoryTypes, CatagoryType>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Childs, property => property.MapFrom(i => i.Tbl_CatagoryTypes1))
                .ForMember(u => u.HasChildren, property => property.MapFrom(i => i.HasChildren))
                .ForMember(u => u.TypeName, property => property.MapFrom(i => i.TypeName))
                .ForMember(u => u.Parent, property => property.MapFrom(i => i.Tbl_CatagoryTypes2))
                .ForMember(u => u.Pid, property => property.MapFrom(i => i.Pid));

            Mapper.CreateMap<CatagoryVendor, Tbl_CatagoryVendors>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Tbl_CatagoryVendors1, property => property.MapFrom(i => i.Childs))
                .ForMember(u => u.HasChildren, property => property.MapFrom(i => i.HasChildren))
                .ForMember(u => u.VendorName, property => property.MapFrom(i => i.VendorName))
                .ForMember(u => u.Tbl_CatagoryVendors2, property => property.MapFrom(i => i.Parent))
                .ForMember(u => u.Pid, property => property.MapFrom(i => i.Pid));

            Mapper.CreateMap<Tbl_CatagoryVendors, CatagoryVendor>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Childs, property => property.MapFrom(i => i.Tbl_CatagoryVendors1))
                .ForMember(u => u.HasChildren, property => property.MapFrom(i => i.HasChildren))
                .ForMember(u => u.VendorName, property => property.MapFrom(i => i.VendorName))
                .ForMember(u => u.Parent, property => property.MapFrom(i => i.Tbl_CatagoryVendors2))
                .ForMember(u => u.Pid, property => property.MapFrom(i => i.Pid));

            Mapper.CreateMap<Files, Tbl_File>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.DataStream, property => property.MapFrom(i => i.DataStream))
                .ForMember(u => u.ItemFileName, property => property.MapFrom(i => i.ItemFileName))
                .ForMember(u => u.Tbl_Items, property => property.MapFrom(i => i.Items));

            Mapper.CreateMap<Tbl_File, Files>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.DataStream, property => property.MapFrom(i => i.DataStream))
                .ForMember(u => u.ItemFileName, property => property.MapFrom(i => i.ItemFileName))
                .ForMember(u => u.Items, property => property.MapFrom(i => i.Tbl_Items));

            Mapper.CreateMap<Tbl_Requests, Requests>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Item, property => property.MapFrom(i => i.Tbl_Items))
                .ForMember(u => u.ItemId, property => property.MapFrom(i => i.ItemId))
                .ForMember(u => u.Message, property => property.MapFrom(i => i.Message))
                .ForMember(u => u.Quantity, property => property.MapFrom(i => i.Quantity))
                .ForMember(u => u.TotalPrice, property => property.MapFrom(i => i.TotalPrice))
                .ForMember(u => u.UnitPrice, property => property.MapFrom(i => i.UnitPrice))
                .ForMember(u => u.User, property => property.MapFrom(i => i.Tbl_Users))
                .ForMember(u => u.UserId, property => property.MapFrom(i => i.UserId));

            Mapper.CreateMap<Requests, Tbl_Requests>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Tbl_Items, property => property.MapFrom(i => i.Item))
                .ForMember(u => u.ItemId, property => property.MapFrom(i => i.ItemId))
                .ForMember(u => u.Message, property => property.MapFrom(i => i.Message))
                .ForMember(u => u.Quantity, property => property.MapFrom(i => i.Quantity))
                .ForMember(u => u.TotalPrice, property => property.MapFrom(i => i.TotalPrice))
                .ForMember(u => u.UnitPrice, property => property.MapFrom(i => i.UnitPrice))
                .ForMember(u => u.Tbl_Users, property => property.MapFrom(i => i.User))
                .ForMember(u => u.UserId, property => property.MapFrom(i => i.UserId));

            Mapper.CreateMap<Item, Tbl_Items>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Added, property => property.MapFrom(i => i.Added))
                .ForMember(u => u.AmountInStock, property => property.MapFrom(i => i.AmountInStock))
                .ForMember(u => u.CatalogNumber, property => property.MapFrom(i => i.CatalogNumber))
                .ForMember(u => u.ContactInfo, property => property.MapFrom(i => i.ContactInfo))
                .ForMember(u => u.Tbl_File, property => property.MapFrom(i => i.Files))
                .ForMember(u => u.IsDeleted, property => property.MapFrom(i => i.IsDeleted))
                .ForMember(u => u.ItemName, property => property.MapFrom(i => i.ItemName))
                .ForMember(u => u.Flagged, property => property.MapFrom(i => i.Flagged))
                .ForMember(u => u.ItemNotes, property => property.MapFrom(i => i.ItemNotes))
                .ForMember(u => u.ItemType, property => property.MapFrom(i => i.ItemType))
                .ForMember(u => u.Tbl_Locations, property => property.MapFrom(i => i.Location))
                .ForMember(u => u.LocationComments, property => property.MapFrom(i => i.LocationComments))
                .ForMember(u => u.LocId, property => property.MapFrom(i => i.LocId))
                .ForMember(u => u.Manufacturer, property => property.MapFrom(i => i.Manufacturer))
                .ForMember(u => u.Tbl_Photo, property => property.MapFrom(i => i.Photos))
                .ForMember(u => u.PurchaseDate, property => property.MapFrom(i => i.PurchaseDate))
                .ForMember(u => u.PurchasePrice, property => property.MapFrom(i => i.PurchasePrice))
                .ForMember(u => u.SubId, property => property.MapFrom(i => i.SubId))
                .ForMember(u => u.Tbl_SubLocations, property => property.MapFrom(i => i.SubLocation))
                .ForMember(u => u.Updated, property => property.MapFrom(i => i.Updated))
                .ForMember(u => u.UpdatedBy, property => property.MapFrom(i => i.UpdatedBy))
                .ForMember(u => u.Tbl_Users, property => property.MapFrom(i => i.User))
                .ForMember(u => u.UsrId, property => property.MapFrom(i => i.UsrId))
                .ForMember(u => u.Vendor, property => property.MapFrom(i => i.Vendor))
                .ForMember(u => u.WebAddress, property => property.MapFrom(i => i.WebAddress))
                .ForMember(u => u.MarkedDeleted, property => property.MapFrom(i => i.MarkedDeleted))
                .ForMember(u => u.CheckedOut, property => property.MapFrom(i => i.CheckedOut))
                .ForMember(u => u.CheckedOutById, property => property.MapFrom(i => i.CheckedOutById));

            Mapper.CreateMap<Tbl_Items, Item>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Added, property => property.MapFrom(i => i.Added))
                .ForMember(u => u.AmountInStock, property => property.MapFrom(i => i.AmountInStock))
                .ForMember(u => u.CatalogNumber, property => property.MapFrom(i => i.CatalogNumber))
                .ForMember(u => u.ContactInfo, property => property.MapFrom(i => i.ContactInfo))
                .ForMember(u => u.Files, property => property.MapFrom(i => i.Tbl_File))
                .ForMember(u => u.IsDeleted, property => property.MapFrom(i => i.IsDeleted))
                .ForMember(u => u.ItemName, property => property.MapFrom(i => i.ItemName))
                .ForMember(u => u.ItemNotes, property => property.MapFrom(i => i.ItemNotes))
                .ForMember(u => u.Flagged, property => property.MapFrom(i => i.Flagged))
                .ForMember(u => u.ItemType, property => property.MapFrom(i => i.ItemType))
                .ForMember(u => u.Location, property => property.MapFrom(i => i.Tbl_Locations))
                .ForMember(u => u.LocationComments, property => property.MapFrom(i => i.LocationComments))
                .ForMember(u => u.LocId, property => property.MapFrom(i => i.LocId))
                .ForMember(u => u.Manufacturer, property => property.MapFrom(i => i.Manufacturer))
                .ForMember(u => u.Photos, property => property.MapFrom(i => i.Tbl_Photo))
                .ForMember(u => u.PurchaseDate, property => property.MapFrom(i => i.PurchaseDate))
                .ForMember(u => u.PurchasePrice, property => property.MapFrom(i => i.PurchasePrice))
                .ForMember(u => u.SubId, property => property.MapFrom(i => i.SubId))
                .ForMember(u => u.SubLocation, property => property.MapFrom(i => i.Tbl_SubLocations))
                .ForMember(u => u.Updated, property => property.MapFrom(i => i.Updated))
                .ForMember(u => u.UpdatedBy, property => property.MapFrom(i => i.UpdatedBy))
                .ForMember(u => u.User, property => property.MapFrom(i => i.Tbl_Users))
                .ForMember(u => u.UsrId, property => property.MapFrom(i => i.UsrId))
                .ForMember(u => u.Vendor, property => property.MapFrom(i => i.Vendor))
                .ForMember(u => u.WebAddress, property => property.MapFrom(i => i.WebAddress))
                .ForMember(u => u.MarkedDeleted, property => property.MapFrom(i => i.MarkedDeleted))
                .ForMember(u => u.CheckedOut, property => property.MapFrom(i => i.CheckedOut))
                .ForMember(u => u.CheckedOutById, property => property.MapFrom(i => i.CheckedOutById));

            Mapper.CreateMap<Location, Tbl_Locations>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Tbl_Items, property => property.MapFrom(i => i.Items))
                .ForMember(u => u.LocationName, property => property.MapFrom(i => i.LocationName))
                .ForMember(u => u.Tbl_SubLocations, property => property.MapFrom(i => i.SubLocations));

                
            Mapper.CreateMap<Tbl_Locations, Location>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Items, property => property.MapFrom(i => i.Tbl_Items))
                .ForMember(u => u.LocationName, property => property.MapFrom(i => i.LocationName))
                .ForMember(u => u.SubLocations, property => property.MapFrom(i => i.Tbl_SubLocations));

            Mapper.CreateMap<SubLocation, Tbl_SubLocations>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Tbl_Items, property => property.MapFrom(i => i.Items))
                .ForMember(u => u.Tbl_Locations, property => property.MapFrom(i => i.Location))
                .ForMember(u => u.LocId, property => property.MapFrom(i => i.LocId))
                .ForMember(u => u.SubLocationName, property => property.MapFrom(i => i.SubLocationName));

            Mapper.CreateMap<Tbl_SubLocations, SubLocation>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.Items, property => property.MapFrom(i => i.Tbl_Items))
                .ForMember(u => u.Location, property => property.MapFrom(i => i.Tbl_Locations))
                .ForMember(u => u.LocId, property => property.MapFrom(i => i.LocId))
                .ForMember(u => u.SubLocationName, property => property.MapFrom(i => i.SubLocationName));

            Mapper.CreateMap<Photos, Tbl_Photo>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.DataStream, property => property.MapFrom(i => i.DataStream))
                .ForMember(u => u.PhotoName, property => property.MapFrom(i => i.PhotoName))
                .ForMember(u => u.Tbl_Items, property => property.MapFrom(i => i.Items))
                .ForMember(u => u.SubId, property => property.MapFrom(i => i.SubId)); 

            Mapper.CreateMap<Tbl_Photo, Photos>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.DataStream, property => property.MapFrom(i => i.DataStream))
                .ForMember(u => u.PhotoName, property => property.MapFrom(i => i.PhotoName))
                .ForMember(u => u.Items, property => property.MapFrom(i => i.Tbl_Items))
                .ForMember(u => u.SubId, property => property.MapFrom(i => i.SubId));

            Mapper.CreateMap<Role, Tbl_Roles>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.RoleName, property => property.MapFrom(i => i.RoleName))
                .ForMember(u => u.Tbl_Users, property => property.MapFrom(i => i.Users));

            Mapper.CreateMap<Tbl_Roles, Role>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.RoleName, property => property.MapFrom(i => i.RoleName))
                .ForMember(u => u.Users, property => property.MapFrom(i => i.Tbl_Users));

            Mapper.CreateMap<User, Tbl_Users>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.IsActive, property => property.MapFrom(i => i.IsActive))
                .ForMember(u => u.Tbl_Items, property => property.MapFrom(i => i.Items))
                .ForMember(u => u.LastName, property => property.MapFrom(i => i.LastName))
                .ForMember(u => u.PasswordHashed, property => property.MapFrom(i => i.PasswordHashed))
                .ForMember(u => u.ActivationCode, property => property.MapFrom(i => i.ActivationCode))
                .ForMember(u => u.Email, property => property.MapFrom(i => i.Email))
                .ForMember(u => u.FirstName, property => property.MapFrom(i => i.FirstName))
                .ForMember(u => u.RoleId, property => property.MapFrom(i => i.RoleId))
                .ForMember(u => u.Tbl_Roles, property => property.MapFrom(i => i.Roles))
                .ForMember(u => u.UserName, property => property.MapFrom(i => i.UserName));

            Mapper.CreateMap<Tbl_Users, User>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.IsActive, property => property.MapFrom(i => i.IsActive))
                .ForMember(u => u.Items, property => property.MapFrom(i => i.Tbl_Items))
                .ForMember(u => u.LastName, property => property.MapFrom(i => i.LastName))
                .ForMember(u => u.PasswordHashed, property => property.MapFrom(i => i.PasswordHashed))
                .ForMember(u => u.ActivationCode, property => property.MapFrom(i => i.ActivationCode))
                .ForMember(u => u.Email, property => property.MapFrom(i => i.Email))
                .ForMember(u => u.FirstName, property => property.MapFrom(i => i.FirstName))
                .ForMember(u => u.RoleId, property => property.MapFrom(i => i.RoleId))
                .ForMember(u => u.Roles, property => property.MapFrom(i => i.Tbl_Roles))
                .ForMember(u => u.UserName, property => property.MapFrom(i => i.UserName));

            Mapper.CreateMap<Tbl_CheckedOut, CheckedOut>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.CheckInDate, property => property.MapFrom(i => i.CheckInDate))
                .ForMember(u => u.CheckOutDate, property => property.MapFrom(i => i.CheckOutDate))
                .ForMember(u => u.FirstName, property => property.MapFrom(i => i.FirstName))
                .ForMember(u => u.LastName, property => property.MapFrom(i => i.LastName))
                .ForMember(u => u.ItemId, property => property.MapFrom(i => i.ItemId))
                .ForMember(u => u.ItemName, property => property.MapFrom(i => i.ItemName))
                .ForMember(u => u.UserId, property => property.MapFrom(i => i.UserId));

            Mapper.CreateMap<CheckedOut, Tbl_CheckedOut>()
                .ForMember(u => u.Id, property => property.MapFrom(i => i.Id))
                .ForMember(u => u.CheckInDate, property => property.MapFrom(i => i.CheckInDate))
                .ForMember(u => u.CheckOutDate, property => property.MapFrom(i => i.CheckOutDate))
                .ForMember(u => u.FirstName, property => property.MapFrom(i => i.FirstName))
                .ForMember(u => u.LastName, property => property.MapFrom(i => i.LastName))
                .ForMember(u => u.ItemId, property => property.MapFrom(i => i.ItemId))
                .ForMember(u => u.ItemName, property => property.MapFrom(i => i.ItemName))
                .ForMember(u => u.UserId, property => property.MapFrom(i => i.UserId));

        }
    }
}