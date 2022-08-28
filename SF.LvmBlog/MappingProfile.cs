using AutoMapper;
using SF.LvmBlog.ViewModels;
using SF.BlogData;
using SF.BlogData.Models;

namespace SF.LvmBlog;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateUserRequest, User>();
        CreateMap<User, UserViewModel>();

        //CreateMap<Article, ArticleShortViewModel>()
            //.ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            //.ForMember(dest => dest.Text, opt => opt.MapFrom(src => 
            //            src.Text.Length > 100 ? src.Text.Substring(0, 100) : src.Text.Substring(0, src.Text.Length)));
        CreateMap<Article, ArticleViewModel>();

        CreateMap<ArticleCreateViewModel, Article>()
           .ForMember(dest => dest.Tags, opt => opt.Ignore());        // Теги установим в методе действия
        CreateMap<Article, ArticleCreateViewModel>()
            .ForMember(dest => dest.Tags, opt => opt.Ignore());

        CreateMap<CommentCreateViewModel, Comment>()
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.CommentText));
        CreateMap<Comment, CommentCreateViewModel>()
            .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.Text)); ;

        CreateMap<User, UserCreateViewModel>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore());
        CreateMap<UserCreateViewModel, User>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        CreateMap<Tag, TagViewModel>();
        CreateMap<TagViewModel, Tag>();

        CreateMap<Role, RoleViewModel>();
        CreateMap<RoleViewModel, Role>();
    }
}
