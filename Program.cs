using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI;
using MinimalAPI.Data;
using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/v1/coupons", (ILogger<Program> _logger) => {
    ApiResponse response = new ();
    _logger.Log(LogLevel.Information, "Getting all coupons.");
    response.Result = CouponStore.couponList;
    response.IsSuccessful = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
   // return Results.Ok(CouponStore.couponList);
    //}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200); 
    }).WithName("GetCoupons").Produces<ApiResponse>(200); 

app.MapGet("/api/v1/coupons/{id:int}", (int id) => {
    ApiResponse response = new();
    response.Result = CouponStore.couponList.FirstOrDefault(u => u.Id == id);
    response.IsSuccessful = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
    //return Results.Ok(CouponStore.couponList.FirstOrDefault(u=>u.Id == id));
}).WithName("GetCoupon").Produces<Coupon>(200);

app.MapPost("/api/v1/coupons", async (IMapper _mapper,
    IValidator <CouponCreateDTO> _validation,
    [FromBody] CouponCreateDTO coupon) => {
        ApiResponse response = new() { IsSuccessful = false, StatusCode = HttpStatusCode.BadRequest};

        var validationResult = await  _validation.ValidateAsync(coupon);
       // var validationResult =  _validation.ValidateAsync(coupon).GetAwaiter().GetResult();

    if(!validationResult.IsValid)
    {
            response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
          return Results.BadRequest(response);
        //return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
    }

        //if (string.IsNullOrEmpty(coupon.Name))
        //{
        //    return Results.BadRequest("Invalid Id or coupon Name");
        //}
    if (CouponStore.couponList.FirstOrDefault(u => u.Name.ToLower() == coupon.Name.ToLower()) !=null)
    {
        return Results.BadRequest("Coupon Name already exists.");
    }

   
    //You could map this way when you don't make use of automapper, using automapper makes the code cleaner.
    //Coupon couponDto = new()
    //{
    //    Name = coupon.Name,
    //    IsActive = coupon.IsActive,
    //    Percent = coupon.Percent,
    //};

    Coupon couponDto = _mapper.Map<Coupon>(coupon);

    couponDto.Id = CouponStore.couponList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
    CouponStore.couponList.Add(couponDto);

    CouponDto couponDto1 = _mapper.Map<CouponDto>(couponDto);

    //You could map this way when you don't make use of automapper, using automapper makes the code cleaner.
    //CouponDto couponDto1 = new()
    //{
    //    Name = couponDto.Name,
    //    IsActive = couponDto.IsActive,
    //    Percent = couponDto.Percent,
    //    Created = couponDto.Created
    //};
    return Results.CreatedAtRoute("GetCoupon",new { couponDto.Id }, couponDto);
    //return Results.Created("$/api/coupon/{coupon.Id}", coupon);
    //return Results.Ok(coupon);
}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<CouponDto>(201).Produces(400);

app.MapPut("/api/v1/coupons", () => {
});

app.MapDelete("/api/v1/coupons/{id:int}", (int id) => {
});





//app.MapGet("/helloworld", () => "Hello World");
//app.MapGet("/helloworld", () =>
//{
//    return Results.BadRequest("An error occured while trying to process the request.");
//});
//app.MapGet("/helloworld/{id}", (int id) =>
//{
//    return Results.Ok("Testing....................");
//  //  return Results.Ok("Id!!!!" + id);
//});
//app.MapPost("helloworld2", () => Results.Ok("Hello world2"));
//app.MapPost("helloworld2", () =>
//{
//    return "HelloWorld";
//});
app.UseHttpsRedirection();
app.Run();

