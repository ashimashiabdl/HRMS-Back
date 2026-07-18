using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class SystemGuideService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<SystemGuide, BaseInfoContext, SystemGuideDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    /// <summary>
    /// Sanitize HTML content to prevent XSS attacks
    /// </summary>
    private string SanitizeHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        // Remove potentially dangerous tags and attributes
        // Allow safe HTML tags for rich text editor (Quill)
        var allowedTags = new[] { "p", "br", "strong", "b", "em", "i", "u", "s", "h1", "h2", "h3", "h4", "h5", "h6", "ul", "ol", "li", "blockquote", "a", "img" };
        var allowedAttributes = new[] { "href", "src", "alt", "title", "style" };

        // Remove script tags and event handlers
        html = Regex.Replace(html, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"on\w+\s*=\s*[""'][^""']*[""']", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"javascript:", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"vbscript:", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"data:", "", RegexOptions.IgnoreCase);

        // Remove style attributes that might contain dangerous content
        html = Regex.Replace(html, @"style\s*=\s*[""'][^""']*expression[^""']*[""']", "", RegexOptions.IgnoreCase);

        return html;
    }

    public new async Task<OperationResult> CreateForAsync(SystemGuideDTO entityToCreate)
    {
        try
        {
            // Sanitize HTML body before saving
            entityToCreate.Body = SanitizeHtml(entityToCreate.Body);

            var mappedEntity = _mapper.Map<SystemGuide>(entityToCreate);
            mappedEntity.title = entityToCreate.title; // Map Title to base title field

            Add(mappedEntity);
            if (await _unitOfWork.Save() > 0)
            {
                return OperationResult.Succeeded(payload: mappedEntity.Id);
            }
            return OperationResult.Failed();
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }

    public new async Task<OperationResult> UpdateForAsync(SystemGuideDTO entityToUpdate)
    {
        try
        {
            if (entityToUpdate.Id == null || entityToUpdate.Id <= 0)
            {
                return OperationResult.Failed("شناسه معتبر نیست");
            }

            // Sanitize HTML body before saving
            entityToUpdate.Body = SanitizeHtml(entityToUpdate.Body);

            var existingEntity = All(false).SingleOrDefault(x => x.Id == entityToUpdate.Id.Value);
            if (existingEntity == null)
            {
                return OperationResult.NotFound();
            }

            var mappedEntity = _mapper.Map<SystemGuide>(entityToUpdate);
            mappedEntity.title = entityToUpdate.title; // Map Title to base title field
            mappedEntity.Id = entityToUpdate.Id.Value;

            Update(mappedEntity);
            if (await _unitOfWork.Save() > 0)
            {
                return OperationResult.Succeeded(payload: 1);
            }
            return OperationResult.Failed();
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }

    public new OperationResult Get(long id)
    {
        try
        {
            var entity = All(false).SingleOrDefault(x => x.Id == id);
            if (entity == null)
            {
                return OperationResult.NotFound();
            }

            var dto = _mapper.Map<SystemGuideDTO>(entity);
            dto.title = entity.title ?? entity.title; // Use Title if available, otherwise use base title
            return OperationResult.Succeeded(payload: dto);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }

    public OperationResult GetByTitle(string title)
    {
        try
        {
            var entity = All(false)
                .Where(x => x.title == title && !x.IsDeleted)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefault();

            if (entity == null)
            {
                return OperationResult.NotFound();
            }

            var dto = _mapper.Map<SystemGuideDTO>(entity);
            dto.title = entity.title ?? entity.title; // Use Title if available, otherwise use base title
            return OperationResult.Succeeded(payload: dto);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }
}
