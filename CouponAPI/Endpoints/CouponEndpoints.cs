using AutoMapper;
using CouponAPI.Data;
using CouponAPI.Models.Dto;
using CouponAPI.Models;
using FluentValidation;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace CouponAPI.Endpoints
{
    public static class CouponEndpoints
    {
        public static void ConfigureCouponEndpoints(this WebApplication app)
        {
            app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
            {
                APIResponse response = new();
                response.Result = CouponStore.couponList;
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;

                _logger.Log(LogLevel.Information, "Getting all Coupons");

                return Results.Ok(response);
            }).WithName("GetCoupons").Produces<APIResponse>(200);

            app.MapGet("/api/coupon/{id:int}", (int id) =>
            {
                APIResponse response = new();
                response.Result = CouponStore.couponList.FirstOrDefault(u => u.Id == id);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;

                return Results.Ok(response);
            }).WithName("GetCoupon").Produces<APIResponse>(200);

            app.MapPost("/api/coupon", async (
                IMapper _mapper,
                IValidator<CouponCreateDTO> _validator,
                [FromBody] CouponCreateDTO coupon_C_DTO) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

                var validationResult = await _validator.ValidateAsync(coupon_C_DTO);
                if (!validationResult.IsValid)
                {
                    response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                    return Results.BadRequest(response);
                }
                if (CouponStore.couponList.FirstOrDefault(u => u.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
                {
                    response.ErrorMessages.Add("Coupon Name already Exists");
                    return Results.BadRequest(response);
                }

                Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

                coupon.Id = CouponStore.couponList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
                coupon.Created = DateTime.Now;

                CouponStore.couponList.Add(coupon);

                CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

                //return Results.CreatedAtRoute("GetCoupon", new { id= couponDTO.Id }, couponDTO);

                response.Result = couponDTO;
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;

                return Results.Ok(response);

            }).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

            app.MapPut("/api/coupon", async (
                IMapper _mapper,
                IValidator<CouponUpdateDTO> _validator,
                [FromBody] CouponUpdateDTO coupon_U_DTO) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

                var validationResult = await _validator.ValidateAsync(coupon_U_DTO);
                if (!validationResult.IsValid)
                {
                    response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                    return Results.BadRequest(response);
                }
                if (CouponStore.couponList.FirstOrDefault(u => u.Name.ToLower() == coupon_U_DTO.Name.ToLower()) != null)
                {
                    response.ErrorMessages.Add("Coupon Name already Exists");
                    return Results.BadRequest(response);
                }

                Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(u => u.Id == coupon_U_DTO.Id);
                couponFromStore.IsActive = coupon_U_DTO.IsActive;
                couponFromStore.Name = coupon_U_DTO.Name;
                couponFromStore.Percent = coupon_U_DTO.Percent;
                couponFromStore.LastUpdated = DateTime.Now;


                response.Result = _mapper.Map<CouponDTO>(couponFromStore);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;

                return Results.Ok(response);

            }).WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

            app.MapDelete("/api/coupon", async (
                IMapper _mapper,
                IValidator<CouponDeleteDTO> _validator,
                [FromBody] CouponDeleteDTO coupon_D_DTO) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

                var validationResult = await _validator.ValidateAsync(coupon_D_DTO);
                if (!validationResult.IsValid)
                {
                    response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                    return Results.BadRequest(response);
                }

                Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(u => u.Id == coupon_D_DTO.Id);
                if (couponFromStore != null)
                {
                    CouponStore.couponList.Remove(couponFromStore);
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.NoContent;
                    return Results.Ok(response);
                }
                else
                {
                    response.ErrorMessages.Add("Invalid Id");
                    return Results.BadRequest(response);
                }
            }).WithName("DeleteCoupon").Accepts<CouponDeleteDTO>("application/json").Produces<APIResponse>(200).Produces(400);

        }
    }
}
