﻿/*
 * This class represents the toolManager object and keeps track of the different tool interfaces and shuffles between them as desired.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that manages the current tool. It runs the drive tool all the time and you can cycle through the tools with buttons.
/// </summary>
public class ToolManager : MonoBehaviour
{
    //Initialize member variables
    public List<ITool> list;
    public GameObject wandObject;
    public GameObject wandControls;
    public GameObject TopLevelUniCAVE;
    public int toolNumber = 0;
    public Text tool;
    public GameObject canvas;
    private DriveTool driveTool;
    public bool negativeAnalogX;
    public bool negativeAnalogY;

    private string TOOL_NAME_PREFIX = "Tool: ";

    /// <summary>
    /// Creates the toolManager object which holds a list of all ITOOL interfaces
    /// </summary>
    /// <param name="wandObject_"></param>
    /// <param name="holder_"></param>
    public ToolManager(GameObject wandObject_, GameObject wandControls_, GameObject TopLevelUniCAVE_, double deadZone, float rotationSpeed, float movementSpeed, Text tool_, bool negateAnalogX_, bool negateAnalogY_)
    {
        wandObject = wandObject_;
        tool = tool_;
        wandControls = wandControls_;
        TopLevelUniCAVE = TopLevelUniCAVE_;
        negativeAnalogX = negateAnalogX_;
        negativeAnalogY = negateAnalogY_;

        list = new List<ITool>();
        
        //fill the list with all the tool interfaces 
        list.Add(wandControls.GetComponent<WarpTool>());
        list.Add(wandControls.GetComponent<GrabberTool>());
        list.Add(wandControls.GetComponent<ButtonTool>());
        list.Add(wandControls.GetComponent<RotatorTool>());

        driveTool = new DriveTool(TopLevelUniCAVE, wandObject, deadZone, rotationSpeed, movementSpeed, negativeAnalogX, negativeAnalogY);

        toolNumber = 0;
        updateToolName(tool);
    }

    /// <summary>
    /// Increments the tool number 
    /// </summary>
    public void NextTool()
    {
        list[toolNumber].shutDown();
        if (toolNumber + 1 >= list.Count)
        {
            toolNumber = 0;
        }
        else
        {
            toolNumber++;
        }
    }

    /// <summary>
    /// Decreases the tool number
    /// </summary>
    public void PreviousTool()
    {
        list[toolNumber].shutDown();
        if (toolNumber - 1 < 0)
        {
            toolNumber = list.Count -1;
        }
        else
        {
            toolNumber--;
        }

    }

    /// <summary>
    /// Handles a button click event. It checks for previous/next click or passes it on to the selected tool. Also calls driver tool for button events.
    /// </summary>
    /// <param name="button">The button clicked</param>
    /// <param name="origin">The tracker position</param>
    /// <param name="direction">The forward direction of the tracker.</param>
    /// <returns></returns>
    public bool handleButtonClick(TrackerButton button, Vector3 origin, Vector3 direction)
    {
        if (button == TrackerButton.NextTool)
        {
            NextTool();
            updateToolName(tool);
            return true;
        }
        else if (button == TrackerButton.PreviousTool)
        {
            PreviousTool();
            updateToolName(tool);
            return true;
        }
        else
        {
            list[toolNumber].ButtonClick(button, origin, direction);
            driveTool.ButtonClick(button, origin, direction);
            return true;
        }
    }


    /// <summary>
    /// Handles a drag event
    /// </summary>
    /// <param name="button">The button dragged</param>
    /// <param name="hit">The object hit</param>
    /// <param name="offset">The offset of the original hit from center of object.</param>
    /// <param name="origin">The tracker position</param>
    /// <param name="direction">The tracker forward direction.</param>
    /// <returns></returns>
    public bool handleButtonDrag(TrackerButton button,RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction)
    {
        list[toolNumber].ButtonDrag(hit, offset, origin, direction);
        return true;
    }

    /// <summary>
    /// Handles the analog changes.
    /// </summary>
    /// <param name="horizontal">The horizontal analog from [-1,1].</param>
    /// <param name="vertical">The vertical analog from [-1,1].</param>
    /// <returns></returns>
    public bool handleAnalog(double horizontal, double vertical)
    {
        driveTool.Analog(horizontal, vertical);
        return true;
    }

    /// <summary>
    /// Updates the display of the tool name on screen.
    /// </summary>
    /// <param name="text">The Text object to update with tool name.</param>
    public void updateToolName(Text text)
    {
        text.text = TOOL_NAME_PREFIX + list[toolNumber].ToolName;
    }

    
}
