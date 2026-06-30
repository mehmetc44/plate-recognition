using AutoMapper;
using PlakaTanima.Application.DTOs.Cameras;
using PlakaTanima.Application.DTOs.Locations;
using PlakaTanima.Application.DTOs.Vehicles;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;
using System;

namespace PlakaTanima.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Vehicle Mappings ---
            CreateMap<Vehicle, VehicleDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => MapCategoryToString(src.Category)))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note ?? ""));

            CreateMap<CreateVehicleDto, Vehicle>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => MapCategory(src.Category)))
                .ForMember(dest => dest.Plate, opt => opt.MapFrom(src => src.Plate.Trim().ToUpper()))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model.Trim()))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner.Trim()))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note != null ? src.Note.Trim() : null));

            CreateMap<UpdateVehicleDto, Vehicle>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => MapCategory(src.Category)))
                .ForMember(dest => dest.Plate, opt => opt.MapFrom(src => src.Plate.Trim().ToUpper()))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model.Trim()))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner.Trim()))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note != null ? src.Note.Trim() : null));

            // --- Camera Mappings ---
            CreateMap<Camera, CameraDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString().ToLower()));

            CreateMap<CreateCameraDto, Camera>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CameraStatus.Offline));

            CreateMap<UpdateCameraDto, Camera>();

            // --- Location Mappings ---
            CreateMap<Location, LocationDto>();
            CreateMap<CreateLocationDto, Location>();
            CreateMap<UpdateLocationDto, Location>();
        }

        private static VehicleCategory MapCategory(string category)
        {
            return category.ToLower() switch
            {
                "vip" => VehicleCategory.Vip,
                "blacklist" => VehicleCategory.Blacklist,
                "staff" => VehicleCategory.Staff,
                _ => VehicleCategory.Normal
            };
        }

        private static string MapCategoryToString(VehicleCategory category)
        {
            return category switch
            {
                VehicleCategory.Vip => "vip",
                VehicleCategory.Blacklist => "blacklist",
                VehicleCategory.Staff => "staff",
                _ => "normal"
            };
        }
    }
}
