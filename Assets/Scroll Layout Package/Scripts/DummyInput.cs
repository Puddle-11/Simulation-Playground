using UnityEngine;

public class DummyInput : MonoBehaviour
{
    [SerializeField] LentorInfiniteScroll scroller;

    InputState state;

    bool initialMove;
    float timer = 0;
    float intervalTimer = 0;
    float moveStartTime = .5f;
    float moveInterval = .25f;
    private void Update()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            state = InputState.Up;
            
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {   
            state = InputState.Down;
        }
        else
        {
            state = InputState.None;
            timer = 0;
            initialMove = false;
        }

        if(state == InputState.Up)
        {
            timer += Time.deltaTime;

            if (!initialMove)
            {
                scroller.Scroll(-10);
                initialMove = true;
            }
            

            if (timer > moveStartTime) 
            {
                intervalTimer = moveInterval;

                if(intervalTimer >= moveInterval)
                {
                    scroller.Scroll(-5);
                    intervalTimer = 0;
                }
            }

        }
        else if (state == InputState.Down)
        {
            timer += Time.deltaTime;
            if (!initialMove)
            {
                scroller.Scroll(10);
                initialMove = true;
            }

            if (timer > moveStartTime)
            {
                intervalTimer = moveInterval;

                if (intervalTimer >= moveInterval)
                {
                    scroller.Scroll(5);
                    intervalTimer = 0;
                }
            }

        }
    }

    public enum InputState
    {
        None, Up, Down, 
    }
}