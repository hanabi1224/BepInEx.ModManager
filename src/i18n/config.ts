import { InitOptions } from 'i18next';
import en from './en';
import zh from './zh';

const i18nConfig: InitOptions = {
    supportedLngs: ['en', 'zh'],
    resources: {
        zh: {
            translation: zh
        },
        en: {
            translation: en
        }
    }
}

export default i18nConfig;
