using System.Collections;

public class Common
{
    private static System.Random random = new System.Random();  

    public static void Shuffle(IList list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = random.Next(n + 1);  
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }
}