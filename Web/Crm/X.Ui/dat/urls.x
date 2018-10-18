^/$->/app.ashx?v=index&t=1
^(/dat/\S+)$->/app.ashx?v=com.err&t=1&p=404
^(/(css|img|js|um|plug)\S+)$->/res/{0}
^/([\w/]+)-?([\S]*)[.]html$->/app.ashx?v={0}&t=1&p={1}
^/(api)/([\d\w.=]+)$->/app.ashx?v={1}&t=2
^/app/getuser-([\w\d]+)-([\w\d]+)-([\w\d/+=]+)-(([\d]+))?(-([\w\d/+=]+))?(.html)?$->/app.ashx?v=getuser&t=1&p={0}-{1}-{2}-{4}-{6}
^/app/([\d\w.=]+)$->/app.ashx?v=app.{0}&t=2