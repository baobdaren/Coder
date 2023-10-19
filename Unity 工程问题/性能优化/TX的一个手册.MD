# TX

* 对于携程的事件等待方式`yield return new WaitForSeconds(1);`，最好提前设置好事件，而不是每一次都去new。

  ```cs
    IEnumerator TestCoroutineProfile()
    {
        var waitObject = new WaitForSeconds(1);
        while (true)
        {
            DoSth();
            yield return waitObject;
            DoSth();
            yield break;
        }
    }
  ```

