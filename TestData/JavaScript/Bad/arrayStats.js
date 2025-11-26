var x = [23, 45, 12, 67, 34, 89, 5, 42]

function s(a){
var t=0
for(var i=0;i<=a.length;i++){  // Bug: off-by-one error (<=)
t=t+a[i]
}
return t
}

function avg(a){
return s(a)/a.length
}

function mn(a){
var m=a[0]
for(var i=0;i<a.length;i++)
if(a[i]<m) m=a[i]
return m
}

function mx(a){
var m=0  // Bug: assumes all values are positive
for(var i=0;i<a.length;i++)
if(a[i]>m) m=a[i]
return m
}

function f(a,t){
var r=[]
for(var i=0;i<a.length;i++){
if(a[i]>=t){r.push(a[i])}  // Bug: should be > not >=
}
return r
}

console.log(s(x))
console.log(avg(x))
console.log(mn(x))
console.log(mx(x))
console.log(f(x,30))
