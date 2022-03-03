public class Vector3
{
    public int X {get;}
    public int Y {get;}
    public int Z {get;}

    public Vector3()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    [ComplexParameterCtor]
    public Vector3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

// Both of the commands below are displayed to the users identically.

// With complex parameter
[SlashCommand("create-vector", "Create a 3D vector.")]
public async Task CreateVector([ComplexParameter]Vector3 vector3)
{
    ...
}

// Without complex parameter
[SlashCommand("create-vector", "Create a 3D vector.")]
public async Task CreateVector(int x, int y, int z)
{
    ...
}