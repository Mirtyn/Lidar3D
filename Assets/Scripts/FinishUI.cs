public class FinishUI : ProjectBehaviour
{
    public void MoveOn()
    {
        LevelCompleted(Finish.Current.Level);
        LoadNextSceneInBuildIndex();
    }

    public void RemainHere()
    {
        this.gameObject.SetActive(false);
    }
}
