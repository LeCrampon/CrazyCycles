using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleSplit : MonoBehaviour
{
    public Rectangle startingRectangle;
    public int panelStep = 2;
    public Vector3 startPosition = Vector3.zero;
    public int nbSteps = 3;
    public List<Rectangle> rectangles;
    public GameObject testCube;

    private void Start()
    {
        rectangles = new List<Rectangle>();
        startingRectangle = new Rectangle();
        startingRectangle.length = 16;
        startingRectangle.width = 16;
        startingRectangle.position = startPosition;
    }


    void SplitRectangle(Rectangle r)
    {
        int vert = Random.Range(0, 2);
        //if(r.length * panelStep < 8 || Random.Range(0,9) == 1)
        //{
        //    return;
        //}  
        if(r.length == 1 || r.width == 1|| Random.Range(0, 9) == 1)
        {
            GameObject cube = Instantiate(testCube, transform);
            cube.transform.position = r.position;
            cube.transform.localScale = new Vector3(r.length, 5, r.width);
            return;
        }
        else
        {
            Rectangle r1 = new Rectangle();
            Rectangle r2 = new Rectangle();
            //r1.length = Random.Range(1, r.length);
            if (vert == 0)
            {
                r1.length = r.length ;
                r1.width = r.width / 2;
                r1.position = new Vector3(r.position.x, r.position.y, r.position.z);

                r2.length = r.length - r1.length;
                r2.width = r.width;
                r1.position = new Vector3(r.position.x + r1.length, r.position.y, r.position.z);
            }
            else
            {
                r1.length = r.length ;
                r1.width = r.width / 2;
                r1.position = new Vector3(r.position.x, r.position.y, r.position.z);

                r2.length = r.length - r1.length;
                r2.width = r.width;
                r1.position = new Vector3(r.position.x , r.position.y, r.position.z + r1.width);
            }
            

            rectangles.Add(r1);
            rectangles.Add(r2);
            SplitRectangle(r1);
            SplitRectangle(r2);
        }
    }

    public void DrawRectangles()
    {
        SplitRectangle(startingRectangle);
    }
    
}
