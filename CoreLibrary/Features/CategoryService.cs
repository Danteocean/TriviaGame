using AutoMapper;
using CoreLibrary.DTOs.Category.Response;
using CoreLibrary.Interface.Services;
using CoreLibrary.Interface.Repositories;
using CoreLibrary.Wrappers;
using Domain.Querys;

namespace CoreLibrary.Features;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Response<List<CategoryDtoResponse>>> GetAllCategoriesAsync()
    {
        try
        {
            var (data, message) = await _unitOfWork.Queries.QueryAsync<dynamic>(SqlQueries.GetAllCategories, null);

            if (data != null && data.Any())
            {
                var result = _mapper.Map<List<CategoryDtoResponse>>(data);
                return new Response<List<CategoryDtoResponse>>(result)
                { State = "Ok", Message = message, Succeeded = true };
            }

            return new Response<List<CategoryDtoResponse>>(null)
            { State = "NoData", Message = message, Succeeded = true };
        }
        catch (Exception ex)
        {
            return new Response<List<CategoryDtoResponse>>(null)
            { State = "Error", Message = ex.Message, Succeeded = false };
        }
    }
}