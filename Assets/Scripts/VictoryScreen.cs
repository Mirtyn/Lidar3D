using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : ProjectBehaviour
{
    private float timeDelta = 0f;
    private float maxTime = 6f;
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;
    float adder = 0f;
    bool run = false;
    public void RunEnd()
    {
        GameObject.FindGameObjectWithTag("TeleportParticleEffect").GetComponent<ParticleSystem>().Play();
        Invoke(nameof(ChangeRunBool), 1.25f);
    }

    private void ChangeRunBool()
    {
        run = !run;
    }

    private void Update()
    {
        if (!Game.GamePaused)
        {
            if (run)
            {
                timeDelta += Time.deltaTime;
                var sum = timeDelta / (maxTime + 0.3f);

                image1.color = new Color(image1.color.r, image1.color.g, image1.color.b, sum - 0.3f + adder);
                image2.color = new Color(image1.color.r, image1.color.g, image1.color.b, sum);

                if (timeDelta > maxTime)
                {
                    adder += Time.deltaTime;

                    if (timeDelta > maxTime + 0.6f)
                    {
                        LoadNextSceneInBuildIndex();
                    }
                }
            }
        }
    }
}
