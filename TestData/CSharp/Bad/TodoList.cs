using System;

class Program{
static string[] t=new string[100];  // fixed array instead of List
static bool[] d=new bool[100];
static int c=0;

static void Main(){
while(true){
Console.WriteLine("1add 2list 3done 4del 5exit");
string x=Console.ReadLine();
if(x=="1"){
Console.Write("todo:");
t[c]=Console.ReadLine();  // no bounds check, crashes after 100 items
d[c]=false;
c++;
}
else if(x=="2"){
for(int i=0;i<c;i++)Console.WriteLine(i+":"+t[i]+(d[i]?"done":""));  // 0-indexed display
}
else if(x=="3"){
int n=int.Parse(Console.ReadLine());  // crashes on non-numeric input
d[n]=true;  // no bounds check
}
else if(x=="4"){
int n=int.Parse(Console.ReadLine());
t[n]=null;  // Bug: doesn't actually remove, leaves holes
d[n]=false;
}
else if(x=="5")break;
}}}
