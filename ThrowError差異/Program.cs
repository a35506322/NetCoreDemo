// See https://aka.ms/new-console-template for more information

try
{
    ThrowNewError();

}
catch (Exception ex)
{
    Console.WriteLine("ThrowNewError"+ ex.ToString());
}

Console.WriteLine();

try
{
    ThrowError();
}
catch (Exception ex)
{
    Console.WriteLine("ThrowError"+ ex.ToString());
}


Console.WriteLine();

try
{
    Throw();
}
catch (Exception ex)
{
    Console.WriteLine("Throw"+ ex.ToString());
}

Console.ReadLine();


static void Funcion ()
{ 
    throw new Exception ("Throw Funcion Error");
}

static void ThrowNewError()
{
    try
    {
        Funcion();
    }
    catch (Exception ex) 
    {
        throw new Exception(ex.ToString());
    }
}

static void ThrowError()
{
    try
    {
        Funcion();
    }
    catch (Exception ex)
    {
        throw ex;
    }
}


static void Throw()
{
    try
    {
        Funcion();
    }
    catch
    {
        throw;
    }
}

