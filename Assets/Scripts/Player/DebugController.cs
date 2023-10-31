using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugController : ProjectBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField outputField;

    public static DebugCommand HELP;
    public static DebugCommand FULL_HEALTH;
    public static DebugCommand KILL_PLAYER;
    public static DebugCommand<float> SET_HEALTH;
    public static DebugCommand<float> SET_MAX_HEALTH;
    public static DebugCommand<float> SET_GAME_SPEED;
    public static DebugCommand LIGHTS;
    public static DebugCommand LOAD_NEXT_LEVEL;
    public static DebugCommand RESTART_LEVEL;
    public static DebugCommand STICKY_VOXELS;

    public List<object> commandList;

    private void Start()
    {
        outputField.text = "Type 'help' for help";
        HELP = new DebugCommand("help", "Shows all the availeable commands", "help", () =>
        {
            ShowHelp();
        });

        FULL_HEALTH = new DebugCommand("full_health", "Sets the players health to full", "full_health", () =>
        {
            Player.Instance.SetPlayerHealthToMax();
        });

        KILL_PLAYER = new DebugCommand("kill_player", "Kills the player", "kill_player", () =>
        {
            Player.Instance.KillPlayer();
        });

        SET_HEALTH = new DebugCommand<float>("set_health", "Sets the players health to the given value", "set_health <health_amount>", (x) =>
        {
            Player.Instance.Health = x;
        });

        SET_MAX_HEALTH = new DebugCommand<float>("set_max_health", "Sets the players max health to the given value", "set_max_health <max_health_amount>", (x) =>
        {
            Player.Instance.MaxHealth = x;
        });

        SET_GAME_SPEED = new DebugCommand<float>("set_game_speed", "Sets the game speed to the given value", "set_game_speed <game_speed>", (x) =>
        {
            ProjectBehaviour.SetGameSpeed(x);
        });

        LIGHTS = new DebugCommand("lights", "Turns the map visible or invisible", "lights", () =>
        {
            Player.Instance.Lights.SetActive(!Player.Instance.Lights.activeSelf);
        });

        LOAD_NEXT_LEVEL = new DebugCommand("load_next_level", "Loads the next level if posible", "load_next_level", () =>
        {
            LoadNextSceneInBuildIndex();
        });

        RESTART_LEVEL = new DebugCommand("restart_level", "Restarts the current level", "restart_level", () =>
        {
            ReLoadCurrentLevel();
        });

        STICKY_VOXELS = new DebugCommand("sticky_voxels", "makes voxels stay on their coresponding object", "sticky_voxels", () =>
        {
            StickyVoxels = !StickyVoxels;
        });

        commandList = new List<object>()
        {
            HELP,
            FULL_HEALTH,
            KILL_PLAYER,
            SET_HEALTH,
            SET_MAX_HEALTH,
            SET_GAME_SPEED,
            LIGHTS,
            LOAD_NEXT_LEVEL,
            RESTART_LEVEL,
            STICKY_VOXELS
        };
    }

    private void Update()
    {
        if (this.isActiveAndEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                outputField.text = inputField.text + "\n" + outputField.text;
                HandleInput();
                inputField.text = "";
            }
        }
    }

    private void HandleInput()
    {
        string[] properties = inputField.text.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (inputField.text.Contains(commandBase.CommandId))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<float> != null)
                {
                    (commandList[i] as DebugCommand<float>).Invoke(int.Parse(properties[1]));
                }
            }
        }
    }

    private void ShowHelp()
    {
        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase command = commandList[i] as DebugCommandBase;

            string label = $"{command.CommandFormat} - {command.CommandDescription}";

            outputField.text = label + "\n" + outputField.text;
        }
    }
}

public class DebugCommandBase
{
    private string commandId;
    private string commandDescription;
    private string commandFormat;

    public string CommandId { get { return commandId;  } }
    public string CommandDescription { get { return commandDescription; } }
    public string CommandFormat { get { return commandFormat; } }

    public DebugCommandBase(string id, string description, string format)
    {
        commandId = id;
        commandDescription = description;
        commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T1> : DebugCommandBase
{
    private Action<T1> command;

    public DebugCommand(string id, string description, string format, Action<T1> command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T1 value)
    {
        command.Invoke(value);
    }
}
