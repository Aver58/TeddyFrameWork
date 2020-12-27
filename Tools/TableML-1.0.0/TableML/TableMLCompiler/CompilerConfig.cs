namespace TableML.Compiler
{
    public class CompilerConfig
    {
        /// <summary>
        /// 编译后的扩展名
        /// </summary>
        public string ExportTabExt = ".tml";
        // 被认为是注释的表头
        public string[] CommentStartsWith = { "Comment", "#" };

        /// <summary>
        /// 定义条件编译指令
        /// </summary>
        public string[] ConditionVars;

        public CompilerConfig()
        {
        }
    }

}
