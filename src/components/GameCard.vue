<template>
    <div>
        <a-spin size="large" :spinning="pending">
            <p>
                <a-button type="primary" size="small" @click="openPath()"> {{'Open Game Folder'|i18n}} </a-button>
                <a-popconfirm
                    v-if="!game.getIsbieinstalled()"
                    :title="confirmInstallBIEMessage"
                    ok-text="Yes"
                    cancel-text="No"
                    @confirm="installBIE"
                >
                    <a-button type="primary" size="small"> {{'Install BIE'|i18n}} </a-button> </a-popconfirm
                ><a-popconfirm
                    v-if="game.getIsbieinstalled()"
                    :title="confirmUninstallBIEMessage"
                    ok-text="Yes"
                    cancel-text="No"
                    @confirm="uninstallBIE"
                >
                    <a-button type="danger" size="small"> {{'Uninstall BIE'|i18n}} </a-button>
                </a-popconfirm>
                <a-button v-if="game.getIsbieinstalled()" type="primary" size="small" @click="openInstallPluginModal">
                    {{'Install Mods'|i18n}}
                </a-button>
                <p v-if="game.getId()">{{'Game Id'|i18n}}: {{ game.getId() }}</p>
                <!-- BIE installed: {{ g.getIsbieinstalled() }},
                initialized {{ g.getIsbieinitialized() }} -->
            </p>
            <div v-if="game.getIsbieinstalled()">
                <div>
                    <a-list
                        item-layout="horizontal"
                        size="large"
                        :data-source="game.getPluginsList()"
                        :locale="listLocale"
                    >
                        <a-list-item :key="item.getName()" slot="renderItem" slot-scope="item">
                            <a-list-item-meta :description="item.getName()">
                                <span slot="title">{{ item.getId() }}(v{{ item.getVersion() }})</span>
                            </a-list-item-meta>
                            <p v-if="item.getDesc()">{{ item.getDesc() }}</p>
                            <p v-if="item.getMiscs()">{{ item.getMiscs() }}</p>
                            <!-- <p>{{ item.getPath() }}</p> -->
                            Actions:
                            <a-popconfirm
                                v-if="game.getIsbieinstalled()"
                                :title="i18n('Are you sure?')"
                                :ok-text="i18n('Yes')"
                                :cancel-text="i18n('No')"
                                @confirm="uninstallPlugin(item)"
                            >
                                <a-button type="danger" size="small"> {{'Uninstall'|i18n}} </a-button> </a-popconfirm
                            ><a-popconfirm
                                v-if="item.getHasupdate()"
                                :title="i18n('Are you sure?')"
                                :ok-text="i18n('Yes')"
                                :cancel-text="i18n('No')"
                                @confirm="installPlugin(item.getUpgradepath())"
                            >
                                <a-button type="primary" size="small"> {{'Upgrade'|i18n}} </a-button></a-popconfirm
                            >
                            <a-popconfirm
                                v-if="item.getMissingreference()"
                                title="Are you sure?"
                                ok-text="Yes"
                                cancel-text="No"
                                @confirm="installPlugin(item.getPath())"
                            >
                                <a-button type="danger" size="small">
                                    {{'Install Missing Dependencies'|i18n}}
                                </a-button></a-popconfirm
                            >
                        </a-list-item>
                    </a-list>
                </div>
            </div>
        </a-spin>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import i18next from 'i18next';
import { Error } from 'grpc-web';
import { grpcClient } from './../utils';
import { GameInfo, InstallBIERequest, PluginInfo, UninstallBIERequest } from './../generated/Game_pb';
import { shell } from 'electron';
import { InstallPluginRequest, UninstallPluginRequest } from '../generated/Repo_pb';
import { CommonServiceResponse } from '../generated/Common_pb';

@Component({
    components: {},
})
export default class GameCard extends Vue {
    @Prop()
    game!: GameInfo;

    pending = false;

    openPath() {
        return shell.openPath(this.game.getPath());
    }

    get confirmInstallBIEMessage() {
        return `Install BIE for ${this.game.getName()}`;
    }

    get confirmUninstallBIEMessage() {
        return `Uninstall BIE for ${this.game.getName()}`;
    }

    get listLocale() {
        return {
            emptyText: i18next.t('No plugins installed'),
        };
    }

    handleCommonResponse(err: Error, response: CommonServiceResponse) {
        try {
            if (err) {
                this.$message.error(err.message);
            } else if (response.getSuccess()) {
                this.refreshGames();
                this.$message.success('Done');
            } else {
                this.$message.warn(response.getError());
            }
        } finally {
            this.pending = false;
        }
    }

    installBIE() {
        this.pending = true;
        const request = new InstallBIERequest().setGamepath(this.game.getPath());
        grpcClient.installBIE(request, {}, this.handleCommonResponse);
    }

    uninstallBIE() {
        this.pending = true;
        const request = new UninstallBIERequest().setGamepath(this.game.getPath());
        grpcClient.uninstallBIE(request, {}, this.handleCommonResponse);
    }

    refreshGames() {
        this.$emit('refreshGames');
    }

    openInstallPluginModal() {
        this.$emit('installPlugin', this.game);
    }

    installPlugin(path: string) {
        console.log(`installPlugin: ${path}`);
        this.pending = true;
        const request = new InstallPluginRequest().setGamepath(this.game!.getPath()).setPluginpath(path);
        grpcClient.installPlugin(request, {}, this.handleCommonResponse);
    }

    uninstallPlugin(p: PluginInfo) {
        this.pending = true;
        const request = new UninstallPluginRequest().setPluginpath(p.getPath());
        grpcClient.uninstallPlugin(request, {}, this.handleCommonResponse);
    }

    todo() {
        this.$message.warning(`Not implemented yet!`);
    }

    i18n(text) {
        i18next.t(text);
    }

    created() {
        i18next.on('languageChanged', (lng) => {
            this.$forceUpdate();
        });
    }
}
</script>

<style lang="scss" scoped>
</style>
