import math
import random

def generate_variations(num_variations=150, fuzziness=1.0):
    # Nejdůležitejší znaky
    chars = (
        [' '] * 5 +
        ['u'] * 3 + ['i'] * 3 + ['e'] * 3 + ['a'] * 3 +
        ['q'] * 2 + ['n'] * 2 + ['s'] * 2 + ['l'] * 2 + ['t'] * 2 +
        ['r'] * 2 + ['o'] * 2 + ['m'] * 2 + ['c'] * 2 + ['d'] * 2 +
        list(".vpgbChjfAPVNMwIQSDEJLUFTOR")
    )

    unique_strings = set()
    attempts = 0
    max_attempts = num_variations * 10 # proti cyklům

    # počet variací
    while len(unique_strings) < num_variations and attempts < max_attempts:
        grid_positions = []
        
        for row in range(8):
            for col in range(8):
                # geometrická vzdálenost od středu matice
                base_dist = math.sqrt((row - 3.5)**2 + (col - 3.5)**2)
                
                # jitter
                # Čím vyšší fuzziness, tím víc znaky budou mimo pozice řetězce:
                # FIjmcfQTSclaqtdDAte uerPdqu  un.vni  ispVraieaoNEgoslmbJOLMChwUR - ten by měl být docela dobrej, bere to jako heatmapu
                noise = random.uniform(-fuzziness, fuzziness)
                fuzzed_dist = base_dist + noise
                
                grid_positions.append((fuzzed_dist, row * 8 + col))
        
        # Sort
        grid_positions.sort()
        sorted_target_indices = [idx for dist, idx in grid_positions]

        # Výslednej string
        result_array = [None] * 64
        for i in range(64):
            result_array[sorted_target_indices[i]] = chars[i]
            
        variation_string = "".join(result_array)
        unique_strings.add(variation_string)
        attempts += 1

    return list(unique_strings)

# --- Spuštění a test ---
if __name__ == "__main__":
    # num_variations - počet výsledků, fuzziness - už jem popsal na řádku 28
    results = generate_variations(num_variations=1000, fuzziness=1.2)

    for result in results:
        print(result)