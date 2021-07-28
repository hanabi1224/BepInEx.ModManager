import Vue from 'vue';
import Antd from 'ant-design-vue';
import VueMeta from 'vue-meta';
import App from './App.vue';
import i18next from 'i18next';
import i18nConfig from './i18n/config';
import osLocale from 'os-locale';
import { ipcRenderer } from 'electron';

i18next.init(i18nConfig)

osLocale().then(async () => {
    const locale = await osLocale();
    i18next.changeLanguage(locale.split('-')[0]);
}
);

ipcRenderer.on('set-lang', (event, lang) => {
    i18next.changeLanguage(lang)
})

Vue.use(Antd)
Vue.use(VueMeta)

Vue.config.productionTip = false;

Vue.prototype.$message.config({
    top: `20vh`,
    duration: 2,
    maxCount: 2,
});

Vue.filter('i18n', function (text) {
    return i18next.t(text)
})

new Vue({
    render: h => h(App),
}).$mount('#app')
