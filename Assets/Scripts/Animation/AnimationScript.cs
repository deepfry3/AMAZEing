using UnityEngine;
using System.Collections;

/* Authors: Imported Asset
 * 
 * AnimationScript is used on the gems in order to make them rotate
 */

public class AnimationScript : MonoBehaviour {

	#region Variables/Properties

    // -- Public -- 
	public bool isAnimated = false;     // Will the object be animating

    public bool isRotating = false;     // Will the object be rotating     
    public bool isFloating = false;     // Will the object be floating
    public bool isScaling = false;      // Will the object be scaling

    public Vector3 rotationAngle;       // The angles in which the object will be rotating
    public float rotationSpeed;         // The speed of the rotation

    public float floatSpeed;            // The speed in which the object is floating
    public float floatRate;             // The amount of times it floats
   
    public Vector3 startScale;          // The begining scale of the object before it scales up
    public Vector3 endScale;            // The final scale of the object once it scales up

    public float scaleSpeed;            // The Speed in which the object changes scale
    public float scaleRate;             // The amount the object will change scale
    
    // -- Private -- 
    private bool goingUp = true;        // Is the object going up
    private float floatTimer;           // The Timer for floating keeping track of time

    private bool scalingUp = true;      // Is the object scaling up
    private float scaleTimer;           // The timer for scaling keepting track of scakling time

	#endregion

	#region Unity Functions
	/// <summary>
    /// Called once per frame.
    /// changes variables and runs the movements of the object
    /// </summary>
	void Update () {

        // If the object is set to animate
        if(isAnimated)
        {
            // If the object is set to rotate
            if(isRotating)
            {
                transform.Rotate(rotationAngle * rotationSpeed * Time.deltaTime);
            }

            // If the object is set to float
            if(isFloating)
            {
                floatTimer += Time.deltaTime;
                Vector3 moveDir = new Vector3(0.0f, 0.0f, floatSpeed);
                transform.Translate(moveDir);

                // Floats down
                if (goingUp && floatTimer >= floatRate)
                {
                    goingUp = false;
                    floatTimer = 0;
                    floatSpeed = -floatSpeed;
                }
                
                // Floats up
                else if(!goingUp && floatTimer >= floatRate)
                {
                    goingUp = true;
                    floatTimer = 0;
                    floatSpeed = +floatSpeed;
                }
            }
            
            // If the object is set to scale
            if(isScaling)
            {
                scaleTimer += Time.deltaTime;

                // Scales up
                if (scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, endScale, scaleSpeed * Time.deltaTime);
                }
                
                // Scales down
                else if (!scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, startScale, scaleSpeed * Time.deltaTime);
                }

                // Toggles scaling 
                if(scaleTimer >= scaleRate)
                {
                    if (scalingUp) { scalingUp = false; }
                    else if (!scalingUp) { scalingUp = true; }
                    scaleTimer = 0;
                }
            }
        }
	}
	#endregion
}
