namespace Filmofil.Services.Repositories
{
    public enum Neo4JRelationships
    {
        HAS_POST = 1,       //(CATEGORIE -> POST)
        HAS_USER = 2,       //(CATEGORIE -> USER)
        POST_AUTHOR = 3,    //(POST -> USER) 
        HAS_COMMENT = 4,    //(POST -> COMMENT) 
        COMMENT_AUTHOR = 5, //(COMMENT -> USER) 
        REPLY = 6,          //(COMMENT -> COMMENT) 
        COMMENT_LIKE = 7,   //(USER -> COMMENT)
        COMMENT_DISLIKE = 8,//(USER -> COMMENT)
        POST_LIKE = 9,      //(USER -> POST) 
        POST_DISLIKE = 10,  //(USER -> POST) 
        PARENT_COMMENT = 11,  //COMMENT(reply) -> COMMENT(parent)
        MY_POST = 12,        //COMMENT -> POST
    }
}
