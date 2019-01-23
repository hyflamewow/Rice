import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppSettingsService } from 'src/app/services/app-settings.service';

@Component({
  selector: 'app-values',
  templateUrl: './values.component.html',
  styleUrls: ['./values.component.scss']
})
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
  constructor(
    appsettingsService: AppSettingsService,
    private http: HttpClient) {
    this.apiUrlRoot = appsettingsService.AppSettings.ApiUrlRoot;
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
