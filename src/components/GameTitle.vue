<template>
  <div>
    <a-avatar
      v-if="game.getIcon()"
      :src="game.getIcon()"
    ></a-avatar>
    {{ game.getName() }} ({{ game.getIs64bit() ? '64bit' : '32bit' | i18n }}) - BIE
    <a-icon
      :type="bieStatusIcon"
      :style="bieStatusIconStyle"
    ></a-icon>
    <span v-if="game.getBieversion()">{{ game.getBieversion() }}</span>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import i18next from 'i18next';
import { GameInfo } from './../generated/Game_pb';

@Component({
    components: {},
})
export default class GameTitle extends Vue {
    @Prop()
    game!: GameInfo;

    get bieStatusIcon() {
        return this.game.getIsbieinstalled() ? 'check' : 'close';
    }

    get bieStatusIconStyle() {
        if (!this.game.getIsbieinstalled()) {
            return 'color:red';
        }
        if (this.game.getIsbieinitialized()) {
            return 'color:green';
        }
        return 'color:orange';
    }

    created() {
        i18next.on('languageChanged', (lng) => {
            this.$forceUpdate();
        });
    }
}
</script>

<style>
</style>
