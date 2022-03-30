let app = new Vue({
    el: '#app',
    data: {
        firstItem: {
            price: 0,
            weight: 0
        },
        secondItem: {
            price: 0,
            weight: 0
        },
    },
    computed: {
        firstPricePerWeight: function () {
            let res = +this.firstItem.price / +this.firstItem.weight;
            return res.toFixed(2);
        },
        secondPricePerWeight: function () {
            let res = +this.secondItem.price / +this.secondItem.weight;
            return res.toFixed(2);
        },
        profit: function () {
            let first = +this.firstPricePerWeight;
            let second = +this.secondPricePerWeight;

            let best = Math.max(first, second);
            let worse = Math.min(first, second);
            
            if(best === worse) {
                return {
                    text: 'Разницы между товарами нет',
                    percent: 0
                };
            }

            let stuff = first < second ? this.firstItem : this.secondItem;

            let percent = Math.abs((worse - best) / best * 100).toFixed();

            let selectedStuff = stuff === this.firstItem ? "Первый" : "Второй";
            return {
                text: `${selectedStuff} вариант выгоднее на ${percent}%`,
                percent: percent
            };
        }
    }
})