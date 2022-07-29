﻿using AutoMapper;
using SF.LvmBlog.ViewModels;
using SF.BlogData;
using SF.BlogData.Models;

namespace SF.LvmBlog;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateUserRequest, User>();

        //CreateMap<Comment, CommentView>();
        //CreateMap<CreateCommentRequest, Comment>();
        //CreateMap<EditCommentRequest, Comment>();

        //CreateMap<Article, ArticleView>()
        //    .ForMember(dest => dest.Author, opt => opt.MapFrom(src => $"{src.Author.Name} (ID: {src.Author.Id}, Login: {src.Author.Login})"))
        //    .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)))
        //    .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments.Select(t => $"[{t.AuthorId}] {t.Text}")));
        //CreateMap<CreateArticleRequest, Article>();
    }
}
