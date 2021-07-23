<template>
  <div>
    <p>
      <a-button
        type="primary"
        size="small"
        @click="openPath()"
      >
        Open Game Folder
      </a-button>
      <a-popconfirm
        :title="confirmInstallBIEMessage"
        ok-text="Yes"
        cancel-text="No"
        @confirm="installBIE"
      >
        <a-button
          v-if="!pending && !game.getIsbieinstalled()"
          type="primary"
          size="small"
        >
          Install BIE
        </a-button>
      </a-popconfirm>
      <a-button
        v-if="!pending && game.getIsbieinstalled()"
        type="danger"
        size="small"
        @click="uninstallBIE()"
      >
        Uninstall BIE
      </a-button>
      <a-button
        v-if="pending"
        size="small"
        disabled="disabled"
      >
        Pending
      </a-button>
      <a-button
        v-if="game.getIsbieinstalled()"
        type="primary"
        size="small"
        @click="todo"
      >
        Install a plugin
      </a-button>
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
          <a-list-item
            :key="item.getName()"
            slot="renderItem"
            slot-scope="item"
          >
            <a-list-item-meta
              :description="item.getName()"
            >
              <span slot="title">{{ item.getId() }}(v{{ item.getVersion() }})</span>
            </a-list-item-meta>  
            <p>{{ item.getPath() }}</p>
            Actions:
            <a-button
              type="primary"
              size="small"
              @click="todo"
            >
              Upgrade
            </a-button> 
            <a-button
              type="danger"
              size="small"
              @click="todo"
            >
              Uninstall
            </a-button> 
          </a-list-item>          
        </a-list>
      </div>
      <!-- <div>
        Installed patchers:
        <p
          v-for="p in game.getPatchersList()"
          :key="p.getId()"
        >
          {{ p.getId() }}({{ p.getVersion() }}) - {{ p.getName() }}
        </p>
      </div> -->
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import i18next from 'i18next';
import { grpcClient } from './../utils';
import { GameInfo, InstallBIERequest } from './../generated/Game_pb';
import { shell } from 'electron';

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

    get listLocale(){
        return {
            'emptyText': i18next.t('No plugins installed'),
        }
    }

    installBIE() {
        this.pending = true;
        const request = new InstallBIERequest().setPath(this.game.getPath());
        grpcClient.installBIE(request, {}, (err, response) => {
            this.pending = false;
            if (response.getSuccess()) {
                this.refreshGames();
            }
        });
    }

    uninstallBIE() {
        this.$message.info(`Open game folder and then delete the BepInEx folder inside`);
    }

    refreshGames() {
        this.$emit('refreshGames');
    }

    todo(){
        this.$message.warning(`Not implemented yet!`);
    }
}
</script>

<style>
</style>
