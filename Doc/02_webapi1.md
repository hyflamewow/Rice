# Rice
2019/01/23
## 目標
使用WebAPI及使用Route

## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab starter
Moon$ npm i
Sun$ dotnet restore
```
## 除錯模式
```shell
Sun$ dotnet run
Moon$ npm run startC
```
瀏覽 http://localhost:4200/

**程式不用關閉, 直接修改程式**
## 修改Moon
## 新增Module及Component
```shell
Moon$ ng g m totem --routing
Moon$ ng g c totem
Moon$ ng g c totem/values
```
### 設定 Router
為避免一次載入太多程式, 所以一開始就採用Lazy Loading。  
app-routing.module.ts
```ts
const routes = [
  { path: '', redirectTo: '/totem', pathMatch: 'full' },
  { path: 'totem', loadChildren: './totem/totem.module#TotemModule' }
];
```
因為Angular的redirectTo只會發生一次, 上面的redirect到/totem,
子路由就不會再發生, 所以直接給預設的component就好了。  
totem/totem-routing.module.ts
```ts
const routes = [
  {
    path: '', component: TotemComponent, children: [
      { path: '', component: ValuesComponent },
      { path: 'values', component: ValuesComponent },
    ]
  }
];
```
### app.component.html
```html
<router-outlet></router-outlet>
```
### totem/totem.component.html
```html
<router-outlet></router-outlet>
```
### 載入HttpClientModuel  
totem/totem.module.ts  
```ts
import { HttpClientModule } from '@angular/common/http';
...
@NgModule({
  ...
  imports: [
    ...
    HttpClientModule
  ],
  ...
})
```
### 建立values component 及 service
```shell
Moon$ ng g c values
Moon\src\app\values$ ng g s values
```
### totem\values\values.component.ts
Visual studio Code可用Ctrl+K, Ctrl+2將region折疊
```ts
export class ValuesComponent implements OnInit {
  //#region 設定及旗標
  private apiUrlRoot: string;
  //#endregion 設定及旗標
  //#region 原始參考資料
  //#region 資料繫結參考列表
  list: string[];
  //#endregion 資料繫結參考列表
  //#region 首要物件
  //#endregion 首要物件
  //#region 屬性
  //#region 初始
  constructor(private http: HttpClient) {
    this.apiUrlRoot = 'api/';
  }
  ngOnInit() {
    this.listAll();
  }
  //#endregion 初始
  //#region 事件處理
  //#region 私有函式
  private listAll() {
    this.http.get<string[]>(`${this.apiUrlRoot}values`)
      .subscribe(list => {
        this.list = list;
      });
  }
  //#endregion 私有函式
  //#region 客製化驗證
}
```
### values.component.html
```html
<ul>
  <li *ngFor="let item of list">
    {{item}}
  </li>
</ul>
```
## 版控
```
Rice$ git add .
Rice$ git commit -m "webapi"
Rice$ git tag webapi
```
## 收尾
```
Rice$ git checkout master
Rice$ branch -d lab
```

## 完成品測試
建置專案
```
$ git clone https://github.com/hyflamewow/Rice.git
Rice$ git checkout -b lab webapi
Moon$ npm i
Moon$ npm run build
Sun$ dotnet restore
Sun$ dotnet run
```
瀏覽  
http://localhost:5000/  
https://localhost:5001/

收尾
```
Rice$ git checkout master
Rice$ branch -d lab
```