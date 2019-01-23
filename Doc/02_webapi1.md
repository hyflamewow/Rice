# Dawn
2018/12/04
## 目標
使用WebAPI

## 專案初始
建置專案
```shell
$ git clone https://github.com/hyflamewow/Dawn.git
Dawn$ git checkout -b lab starter
Moon$ npm i
Sun$ dotnet restore
```
## 除錯模式
```shell
Sun$ dotnet run
Moon$ npm start
```
瀏覽 http://localhost:4200/

**程式不用關閉, 直接修改程式**
## 修改Moon
### 載入HttpClientModuel  
src/app/app.module.ts  
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
### src\app\values\values.service.ts
```ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ValuesService {
  constructor(private http: HttpClient) { }
  getList(): Observable<string[]> {
    return this.http.get<string[]>('api/values');
  }
}
```
### values.components.ts
```ts
import { Component, OnInit } from '@angular/core';
import { ValuesService } from './values.service';

@Component({
  selector: 'app-values',
  templateUrl: './values.component.html',
  styleUrls: ['./values.component.scss']
})
export class ValuesComponent implements OnInit {
  list: string[];
  constructor(private valuesService: ValuesService) { }
  ngOnInit() {
    this.getList();
  }
  getList() {
    this.valuesService.getList()
      .subscribe(list => {
        this.list = list;
      });
  }
}
```
### 設定 Router
app-routing.module.ts
```ts
import { ValuesComponent } from './values/values.component';

const routes: Routes = [
  { path: '', redirectTo: '/values', pathMatch: 'full' },
  { path: 'values', component: ValuesComponent }
];
```
### app.component.html
```html
<router-outlet></router-outlet>
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
Dawn$ git add .
Dawn$ git commit -m "webapi"
Dawn$ git tag webapi
```
## 收尾
```
Dawn$ git checkout master
Dawn$ branch -d lab
```

## 完成品測試
建置專案
```
$ git clone https://github.com/hyflamewow/Dawn.git
Dawn$ git checkout -b lab webapi
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
Dawn$ git checkout master
Dawn$ branch -d lab
```