import Vue from 'vue';
import Antd from 'ant-design-vue';
import VueMeta from 'vue-meta';
import App from './App.vue';
import i18next from 'i18next';

Vue.use(Antd)
Vue.use(VueMeta)

Vue.config.productionTip = false;
i18next.init()

new Vue({
    render: h => h(App),
}).$mount('#app')
