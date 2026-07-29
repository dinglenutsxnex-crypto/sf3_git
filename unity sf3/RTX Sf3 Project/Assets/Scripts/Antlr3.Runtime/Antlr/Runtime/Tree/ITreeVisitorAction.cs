namespace Antlr.Runtime.Tree
{
	public interface ITreeVisitorAction
	{
		object Pre(object t);

		object Post(object t);
	}
}
