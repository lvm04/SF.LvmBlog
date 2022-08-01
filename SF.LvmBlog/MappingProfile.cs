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

        CreateMap<Article, ArticleShortViewModel>()
            //.ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => 
                        src.Text.Length > 100 ? src.Text.Substring(0, 100) : src.Text.Substring(0, src.Text.Length)));
        CreateMap<Article, ArticleViewModel>();

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
