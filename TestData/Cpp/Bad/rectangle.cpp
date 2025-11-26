#include <iostream>
using namespace std;

class rect{
public:
double w;
double h;

rect(double a,double b){w=a;h=b;}

double a(){return w*w;}  // Bug: should be w*h

double p(){return 2*w+h;}  // Bug: should be 2*(w+h)
};

int main(){
rect r(5,3);
cout<<r.a()<<endl;
cout<<r.p()<<endl;
r.w=-10;  // No validation, allows negative
cout<<r.a()<<endl;
return 0;
}
